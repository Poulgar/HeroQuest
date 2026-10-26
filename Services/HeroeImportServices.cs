using HeroQuest.Data;
using HeroQuest.Models;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Services;

public class HeroeImportService(ApplicationDbContext context)
{
    public async Task ImportarAsync(string ruta)
    {
        if (!File.Exists(ruta))
            throw new FileNotFoundException("No se encontró el CSV de héroes.", ruta);

        var registros = CsvRecordReader.ReadRecords(ruta);
        var heroesExistentes = await context.Heroes.ToListAsync();
        var importarHeroesNuevos = heroesExistentes.Count == 0;
        var nextId = await context.Heroes.Select(h => (int?)h.Id).MaxAsync() ?? 0;
        nextId = Math.Max(nextId, await context.Monsters.Select(m => (int?)m.Id).MaxAsync() ?? 0) + 1;

        foreach (var datos in registros)
        {
            var nombre = NormalizeClassName(CsvRecordReader.GetText(datos, "Name"));
            var characterTypeClass = NormalizeClassName(CsvRecordReader.GetText(datos, "CharacterTypeClass"));
            var aparicion = TypeAppearsParser.Parse(CsvRecordReader.GetText(datos, "Appearances"));
            var heroeExistente = heroesExistentes.FirstOrDefault(heroe =>
                string.Equals(NormalizeClassName(heroe.Name), nombre, StringComparison.OrdinalIgnoreCase));

            if (heroeExistente is not null)
            {
                // Al volver a importar, sólo sincronizamos la expansión; preservamos el estado de la campaña.
                // La normalización también actualiza filas antiguas que aún conservan el sufijo «/a».
                heroeExistente.Name = nombre;
                heroeExistente.CharacterTypeClass = characterTypeClass;
                heroeExistente.Appears = aparicion;
                continue;
            }

            // Los datos de clase sólo se usan para inicializar una tabla vacía; no se agregan plantillas
            // a una partida que ya contiene héroes con nombres o progresos propios.
            if (!importarHeroesNuevos)
                continue;

            var heroe = new Heroe(
                id: nextId++,
                characterTypeClass: characterTypeClass,
                hp: CsvRecordReader.GetInteger(datos, "Hp"),
                dicesMind: CsvRecordReader.GetInteger(datos, "DicesMind"),
                dicesAttack: CsvRecordReader.GetInteger(datos, "DicesAttack"),
                dicesDefend: CsvRecordReader.GetInteger(datos, "DicesDefend"),
                dicesRedMovement: CsvRecordReader.GetInteger(datos, "DicesRedMovement"),
                description: CsvRecordReader.GetText(datos, "Description"))
            {
                CharacterTypeClass = characterTypeClass,
                Name = nombre,
                Player = CsvRecordReader.GetText(datos, "Player"),
                Weapon = CsvRecordReader.GetText(datos, "Weapon"),
                Armor = CsvRecordReader.GetText(datos, "Armor"),
                Spells = CsvRecordReader.GetText(datos, "Spells"),
                Gold = CsvRecordReader.GetInteger(datos, "Gold"),
                Items = CsvRecordReader.GetText(datos, "Items"),
                QuestsCompleted = CsvRecordReader.GetInteger(datos, "QuestsCompleted"),
                Appears = aparicion
            };

            context.Heroes.Add(heroe);
        }

        await context.SaveChangesAsync();
    }

    private static string NormalizeClassName(string name) => name.Trim() switch
    {
        "Bárbaro/a" => "Bárbaro",
        "Elfo/a" => "Elfo",
        "Enano/a" => "Enano",
        "Mago/a" => "Mago",
        var normalized => normalized
    };
}
