using CsvHelper;
using CsvHelper.Configuration;
using HeroQuest.Data;
using HeroQuest.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace HeroQuest.Services;

public class HeroeImportService(ApplicationDbContext context)
{
    public async Task ImportarAsync(string ruta)
    {
        if (!File.Exists(ruta))
            throw new FileNotFoundException("No se encontró el CSV de héroes.", ruta);

        using var reader = new StreamReader(ruta);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            PrepareHeaderForMatch = args => args.Header.Trim()
        };

        using var csv = new CsvReader(reader, config);
        var registros = csv.GetRecords<dynamic>().ToList();
        var heroesExistentes = await context.Heroes.ToListAsync();
        var importarHeroesNuevos = heroesExistentes.Count == 0;
        var nextId = await context.Heroes.Select(h => (int?)h.Id).MaxAsync() ?? 0;
        nextId = Math.Max(nextId, await context.Monsters.Select(m => (int?)m.Id).MaxAsync() ?? 0) + 1;

        foreach (var fila in registros)
        {
            var datos = (IDictionary<string, object>)fila;
            string Texto(string campo) => datos.TryGetValue(campo, out var valor) ? valor?.ToString()?.Trim() ?? "" : "";
            int Numero(string campo) => int.TryParse(Texto(campo), NumberStyles.Integer, CultureInfo.InvariantCulture, out var valor) ? valor : 0;

            var nombre = Texto("Name");
            var aparicion = TypeAppearsParser.Parse(Texto("Appearances"));
            var heroeExistente = heroesExistentes.FirstOrDefault(heroe =>
                string.Equals(heroe.Name, nombre, StringComparison.OrdinalIgnoreCase));

            if (heroeExistente is not null)
            {
                // Al volver a importar, sólo sincronizamos la expansión; preservamos el estado de la campaña.
                heroeExistente.Appears = aparicion;
                continue;
            }

            // Los datos de clase sólo se usan para inicializar una tabla vacía; no se agregan plantillas
            // a una partida que ya contiene héroes con nombres o progresos propios.
            if (!importarHeroesNuevos)
                continue;

            var heroe = new Heroe(
                id: nextId++,
                characterType: Texto("CharacterType"),
                hp: Numero("Hp"),
                mind: Numero("Mind"),
                dicesAttack: Numero("DicesAttack"),
                dicesDefend: Numero("DicesDefend"),
                dicesRedMovement: Numero("DicesRedMovement"),
                description: Texto("Description"))
            {
                CharacterType = Texto("CharacterType"),
                Name = nombre,
                Player = Texto("Player"),
                Weapon = Texto("Weapon"),
                Armor = Texto("Armor"),
                Spells = Texto("Spells"),
                Gold = Numero("Gold"),
                Items = Texto("Items"),
                QuestsCompleted = Numero("QuestsCompleted"),
                Appears = aparicion
            };

            context.Heroes.Add(heroe);
        }

        await context.SaveChangesAsync();
    }
}
