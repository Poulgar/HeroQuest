using Microsoft.Data.Sqlite;

namespace HeroQuest.Services;

/// <summary>Guarda una copia semanal válida de SQLite y conserva solo las ocho últimas.</summary>
public sealed class DatabaseBackupService(string databasePath, ILogger<DatabaseBackupService> logger) : BackgroundService
{
    private static readonly TimeSpan BackupInterval = TimeSpan.FromDays(7);
    private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(12);
    private const int RetainedCopies = 8;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CreateBackupIfDueAsync(stoppingToken);
                await Task.Delay(CheckInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "No se pudo crear la copia semanal de la base de datos.");
                await Task.Delay(CheckInterval, stoppingToken);
            }
        }
    }

    private async Task CreateBackupIfDueAsync(CancellationToken cancellationToken)
    {
        var backupDirectory = Path.Combine(Path.GetDirectoryName(databasePath)!, "Backups");
        Directory.CreateDirectory(backupDirectory);
        var existingBackups = Directory.GetFiles(backupDirectory, "HeroQuest_weekly_*.db")
            .OrderByDescending(File.GetLastWriteTimeUtc).ToArray();

        // Revisa dos veces al día, pero crea un archivo solo cuando ya han pasado siete días.
        if (existingBackups.Length > 0 && DateTime.UtcNow - File.GetLastWriteTimeUtc(existingBackups[0]) < BackupInterval)
            return;

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var finalPath = Path.Combine(backupDirectory, $"HeroQuest_weekly_{timestamp}.db");
        var temporaryPath = finalPath + ".tmp";

        try
        {
            await using var source = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                DataSource = databasePath,
                Mode = SqliteOpenMode.ReadOnly,
                Pooling = false
            }.ToString());
            await source.OpenAsync(cancellationToken);
            await EnsureDatabaseIsHealthyAsync(source, cancellationToken);

            await using (var destination = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                DataSource = temporaryPath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Pooling = false
            }.ToString()))
            {
                await destination.OpenAsync(cancellationToken);
                source.BackupDatabase(destination);
                await EnsureDatabaseIsHealthyAsync(destination, cancellationToken);
            }

            // Publica el archivo solo después de completar y validar la copia temporal.
            File.Move(temporaryPath, finalPath);
            PruneOldBackups(backupDirectory);
            logger.LogInformation("Copia semanal de HeroQuest creada: {BackupPath}", finalPath);
        }
        catch
        {
            if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
            throw;
        }
    }

    private static async Task EnsureDatabaseIsHealthyAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA integrity_check;";
        var result = await command.ExecuteScalarAsync(cancellationToken);
        if (!string.Equals(result?.ToString(), "ok", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException($"La comprobación de SQLite devolvió: {result ?? "sin resultado"}.");
    }

    private static void PruneOldBackups(string directory)
    {
        var backups = Directory.GetFiles(directory, "HeroQuest_weekly_*.db")
            .OrderByDescending(File.GetLastWriteTimeUtc).ToArray();
        foreach (var oldBackup in backups.Skip(RetainedCopies))
            File.Delete(oldBackup);
    }
}
