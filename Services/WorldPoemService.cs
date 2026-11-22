using HeroQuest.Data;
using HeroQuest.Models;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Services;

public sealed class WorldPoemService(ApplicationDbContext context)
{
    private static readonly WorldPoem[] FirstPoems =
    [
        new() { Key = "gate", Title = "La guardia del Compendio", Text = "Alto, viajero, di tu nombre sin temor;\ny revela tu código para cruzar el umbral." },
        new() { Key = "day", Title = "El libro al alba", Text = "Despierta, vieja crónica, que el sol dora el papel;\nque las hazañas dormidas vuelvan a nacer." },
        new() { Key = "night", Title = "La tinta de la noche", Text = "Cuando la luna vela sobre piedra y rosal,\nque la tinta encantada te guíe en la oscuridad." }
    ];

    // Se añaden solo las claves que falten, así los versos personalizados sobreviven a reinicios.
    public async Task EnsureFirstPoemsAsync()
    {
        var existingKeys = await context.WorldPoems.Select(poem => poem.Key).ToListAsync();
        var missing = FirstPoems.Where(poem => !existingKeys.Contains(poem.Key, StringComparer.OrdinalIgnoreCase));
        context.WorldPoems.AddRange(missing);
        await context.SaveChangesAsync();
    }

    public Task<WorldPoem?> FindAsync(string key)
        => context.WorldPoems.AsNoTracking().FirstOrDefaultAsync(poem => poem.Key == key);
}
