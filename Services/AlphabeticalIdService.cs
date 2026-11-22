using System.Globalization;
using HeroQuest.Data;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Services;

/// <summary>
/// Asigna IDs alfabéticos alternados: Héroes impares y Monstruos pares.
/// Los enlaces monstruo-hechizo deben vaciarse antes de llamar a este servicio y reconstruirse después.
/// </summary>
public sealed class AlphabeticalIdService(ApplicationDbContext context)
{
    private sealed record Row(int Id, string Name, string Table);

    public async Task EnsureAlphabeticalIdsAsync()
    {
        var comparer = StringComparer.Create(CultureInfo.GetCultureInfo("es-ES"), ignoreCase: true);
        var heroes = (await context.Heroes.AsNoTracking().Select(x => new Row(x.Id, x.Name, "Heroes")).ToListAsync())
            .OrderBy(x => x.Name, comparer).ThenBy(x => x.Id).ToList();
        var monsters = (await context.Monsters.AsNoTracking().Select(x => new Row(x.Id, x.Name, "Monsters")).ToListAsync())
            .OrderBy(x => x.Name, comparer).ThenBy(x => x.Id).ToList();
        var miscellaneaEntries = (await context.Miscellanea.AsNoTracking().Select(x => new Row(x.Id, x.Name, "Miscellanea")).ToListAsync())
            .OrderBy(x => x.Name, comparer).ThenBy(x => x.Id).ToList();
        var equipment = (await context.EquipmentsAndArtifacts.AsNoTracking().Select(x => new Row(x.Id, x.Name, "EquipmentsAndArtifacts")).ToListAsync())
            .OrderBy(x => x.Name, comparer).ThenBy(x => x.Id).ToList();
        var abilities = (await context.MonsterAbilities.AsNoTracking().Select(x => new Row(x.Id, x.Name, "MonsterAbilities")).ToListAsync())
            .OrderBy(x => x.Name, comparer).ThenBy(x => x.Id).ToList();
        var spells = (await context.SpellsOfTerror.AsNoTracking().Select(x => new Row(x.Id, x.Name, "SpellsOfTerror")).ToListAsync())
            .OrderBy(x => x.Name, comparer).ThenBy(x => x.Id).ToList();

        var needsReordering = !HasAlternatingIds(heroes, 1)
            || !HasAlternatingIds(monsters, 2)
            || !HasSequentialIds(miscellaneaEntries)
            || !HasSequentialIds(equipment)
            || !HasSequentialIds(abilities)
            || !HasSequentialIds(spells);
        if (!needsReordering) return;

        // El desplazamiento temporal evita que los nuevos IDs choquen con claves todavía ocupadas.
        var allRows = heroes.Concat(monsters).Concat(miscellaneaEntries).Concat(equipment).Concat(abilities).Concat(spells).ToList();
        var offset = (allRows.Count == 0 ? 0 : allRows.Max(x => x.Id)) + allRows.Count + 1;
        await using var transaction = await context.Database.BeginTransactionAsync();

        await ShiftAsync("Heroes", heroes, offset);
        await ShiftAsync("Monsters", monsters, offset);
        await ShiftAsync("Miscellanea", miscellaneaEntries, offset);
        await ShiftAsync("EquipmentsAndArtifacts", equipment, offset);
        await ShiftAsync("MonsterAbilities", abilities, offset);
        await ShiftAsync("SpellsOfTerror", spells, offset);

        // Cada clase conserva su orden alfabético, pero ocupa una paridad distinta y nunca comparte IDs.
        await AssignAlternatingIdsAsync(heroes, offset, firstId: 1);
        await AssignAlternatingIdsAsync(monsters, offset, firstId: 2);

        await AssignSequentialIdsAsync("Miscellanea", miscellaneaEntries, offset);
        await AssignSequentialIdsAsync("EquipmentsAndArtifacts", equipment, offset);
        await AssignSequentialIdsAsync("MonsterAbilities", abilities, offset);
        await AssignSequentialIdsAsync("SpellsOfTerror", spells, offset);
        await transaction.CommitAsync();

        // Los importadores pueden haber dejado entidades rastreadas antes del reordenamiento SQL.
        context.ChangeTracker.Clear();
    }

    private static bool HasAlternatingIds(IReadOnlyList<Row> rows, int firstId)
        => rows.Select((row, index) => row.Id == firstId + index * 2).All(matches => matches);

    private static bool HasSequentialIds(IReadOnlyList<Row> rows)
        => rows.Select((row, index) => row.Id == index + 1).All(matches => matches);

    private async Task AssignAlternatingIdsAsync(IReadOnlyList<Row> rows, int offset, int firstId)
    {
        for (var index = 0; index < rows.Count; index++)
            await SetIdAsync(rows[index].Table, rows[index].Id + offset, firstId + index * 2);
    }

    private async Task AssignSequentialIdsAsync(string table, IReadOnlyList<Row> rows, int offset)
    {
        for (var index = 0; index < rows.Count; index++)
            await SetIdAsync(table, rows[index].Id + offset, index + 1);
    }

    private async Task ShiftAsync(string table, IReadOnlyList<Row> rows, int offset)
    {
        foreach (var row in rows)
            await SetIdAsync(table, row.Id, row.Id + offset);
    }

    private Task<int> SetIdAsync(string table, int oldId, int newId)
    {
        // El nombre de tabla se elige de una lista cerrada; los valores de clave siguen parametrizados.
        var sql = table switch
        {
            "Heroes" => "UPDATE \"Heroes\" SET \"Id\" = {0} WHERE \"Id\" = {1}",
            "Monsters" => "UPDATE \"Monsters\" SET \"Id\" = {0} WHERE \"Id\" = {1}",
            "Miscellanea" => "UPDATE \"Miscellanea\" SET \"Id\" = {0} WHERE \"Id\" = {1}",
            "EquipmentsAndArtifacts" => "UPDATE \"EquipmentsAndArtifacts\" SET \"Id\" = {0} WHERE \"Id\" = {1}",
            "MonsterAbilities" => "UPDATE \"MonsterAbilities\" SET \"Id\" = {0} WHERE \"Id\" = {1}",
            "SpellsOfTerror" => "UPDATE \"SpellsOfTerror\" SET \"Id\" = {0} WHERE \"Id\" = {1}",
            _ => throw new ArgumentOutOfRangeException(nameof(table), table, "Tabla no permitida para reindexar.")
        };
        return context.Database.ExecuteSqlRawAsync(sql, newId, oldId);
    }
}
