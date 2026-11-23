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
        ,new() { Key = "library-guardian", Title = "El guardián de la Biblioteca", Text = "Maestro, avanzo a tu lado entre pergamino y cristal;\nmi vigilia cuida el reino mientras escribes su destino." }
        ,new() { Key = "library-added", Title = "Una nueva crónica", Text = "He inscrito esta ficha en los anaqueles del reino;\nque su nombre perdure donde la memoria lo encuentre." }
        ,new() { Key = "library-edited", Title = "La crónica revisada", Text = "He sellado tus cambios en el libro de campaña;\nla tinta recuerda el rumbo que has elegido." }
        ,new() { Key = "library-archived", Title = "Al resguardo del archivo", Text = "He guardado esta ficha bajo llave;\nsu nombre seguirá vivo, aunque repose fuera de la vista." }
        ,new() { Key = "library-restored", Title = "La ficha vuelve a la luz", Text = "He devuelto esta ficha a los estantes;\nque vuelva a acompañar a quienes recorren el reino." }
        ,new() { Key = "library-destroyed", Title = "La última página", Text = "La ficha ha sido borrada de estos anales;\nvelaré para que su ausencia no quiebre el resto de la crónica." }
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
        => context.WorldPoems.AsNoTracking().FirstOrDefaultAsync(poem => poem.Key == key && !poem.IsArchived);

    public async Task<string> MessageAsync(string key, string name)
    {
        var poem = await FindAsync(key);
        return poem is null ? "El guardián ha sellado la ficha en la Biblioteca." : $"{poem.Title}: {poem.Text.Replace("esta ficha", $"«{name}»", StringComparison.OrdinalIgnoreCase)}";
    }
}
