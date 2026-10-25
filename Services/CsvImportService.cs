using CsvHelper;
using CsvHelper.Configuration;
using HeroQuest.Data;
using HeroQuest.Models;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Services;

// Servicio para importar datos desde archivos CSV a la base de datos
public class CsvImportService
{
    private readonly ApplicationDbContext _context;

    public CsvImportService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Importa todos los archivos CSV ignorando duplicados si ya existen
    public async Task ImportAllAsync(string csvFolderPath)
    {
        var monsterCategoriesAbilitiesPath = Path.Combine(csvFolderPath, "HeroQuest_MonstersCategoriesAbilities.csv");
        var miscellaneaPath = Path.Combine(csvFolderPath, "HeroQuest_Miscellanea.csv");
        var equipmentPath = Path.Combine(csvFolderPath, "HeroQuest_Equipment.csv");
        var artifactsPath = Path.Combine(csvFolderPath, "HeroQuest_Artifacts.csv");
        var abilitiesPath = Path.Combine(csvFolderPath, "HeroQuest_MonsterAbilities.csv");
        var spellsOfTerrorPath = Path.Combine(csvFolderPath, "HeroQuest_SpellsOfTerror.csv");

        // No ocultamos una ruta incorrecta: así la app informa por qué no pudo llenar una tabla.
        foreach (var path in new[] { monsterCategoriesAbilitiesPath, miscellaneaPath, equipmentPath, artifactsPath, abilitiesPath, spellsOfTerrorPath })
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"No se encontró el CSV requerido: {Path.GetFileName(path)}", path);
        }

        await ImportSpellsOfTerrorAsync(spellsOfTerrorPath);
        await ImportMonstersAsync(monsterCategoriesAbilitiesPath);
        await ImportMiscellaneaAsync(miscellaneaPath); // Importa el CSV de objetos a la tabla Miscelánea.
        await ImportEquipmentsAndArtifactsAsync(
            equipmentPath, // Equipamiento
            artifactsPath); // Artefactos
        await ImportMonsterAbilitiesAsync(abilitiesPath); // Habilidades

        // Rehacemos los vínculos tras reordenar IDs para que las claves externas apunten a los IDs finales.
        await _context.MonsterSpellOfTerrors.ExecuteDeleteAsync();
        await new AlphabeticalIdService(_context).EnsureAlphabeticalIdsAsync();
        await ImportMonsterSpellLinksAsync(monsterCategoriesAbilitiesPath);
    }

    private CsvConfiguration GetConfig()
    {
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            PrepareHeaderForMatch = args => args.Header.Trim(),
            BadDataFound = null,
            MissingFieldFound = null
        };
    }

    private async Task ImportMonstersAsync(string path)
    {
        var existingMonsters = await _context.Monsters.ToListAsync();
        var byName = existingMonsters
            .GroupBy(monster => MonsterCsvParsing.NormalizeName(monster.Name))
            .ToDictionary(group => group.Key, group => group.First());

        // Heroes y Monsters comparten la clave de Character en TPC; los nuevos IDs parten del máximo global.
        var maxHeroId = await _context.Heroes.Select(h => (int?)h.Id).MaxAsync() ?? 0;
        var maxMonsterId = await _context.Monsters.Select(m => (int?)m.Id).MaxAsync() ?? 0;
        var nextId = Math.Max(maxHeroId, maxMonsterId) + 1;

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, GetConfig());
        var records = csv.GetRecords<dynamic>().ToList();

        foreach (var record in records)
        {
            var fields = (IDictionary<string, object>)record;
            string Text(string field) => fields.TryGetValue(field, out var value) ? value?.ToString()?.Trim() ?? "" : "";
            int Number(string field) => int.TryParse(Text(field), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : 0;

            var name = Text("Nombre");
            var hp = Number("Cuerpo");
            var mind = Number("Mente");
            var attack = Number("Ataque");
            var defense = Number("Defensa");
            var maxMovement = Number("Movimiento");
            var faction = Text("Categoria");
            var ability = MonsterCsvParsing.CleanAbilityNames(Text("Habilidad"));
            var appearance = TypeAppearsParser.Parse(Text("Apariciones"));

            if (!byName.TryGetValue(MonsterCsvParsing.NormalizeName(name), out var monster))
            {
                monster = new Monster(nextId++, name, hp, mind, attack, defense, maxMovement, ability, faction,
                    CreateDemoDescription(name, faction, hp, maxMovement))
                {
                    Appears = appearance
                };
                _context.Monsters.Add(monster);
                byName[MonsterCsvParsing.NormalizeName(name)] = monster;
                continue;
            }

            // Actualizamos la ficha existente desde el CSV, pero mantenemos la vida actual de una campaña.
            monster.CharacterType = name;
            monster.Name = name;
            monster.Hp = hp;
            monster.Mind = mind;
            monster.DicesAttack = attack;
            monster.DicesDefend = defense;
            monster.MaxMovement = maxMovement;
            monster.Attack = attack;
            monster.Defense = defense;
            monster.Faction = faction;
            monster.Ability = ability;
            monster.Appears = appearance;
            if (string.IsNullOrWhiteSpace(monster.Description))
                monster.Description = CreateDemoDescription(name, faction, hp, maxMovement);
        }

        await _context.SaveChangesAsync();
    }

    private static string CreateDemoDescription(string name, string faction, int hp, int maxMovement)
        => $"{name} pertenece a la facción {faction}. Tiene {hp} puntos de cuerpo y un movimiento máximo de {maxMovement}.";

    private async Task ImportSpellsOfTerrorAsync(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, GetConfig());
        var records = csv.GetRecords<dynamic>().ToList();
        var existing = (await _context.SpellsOfTerror.ToListAsync())
            .GroupBy(spell => MonsterCsvParsing.NormalizeName(spell.Name))
            .ToDictionary(group => group.Key, group => group.First());

        foreach (var record in records)
        {
            var fields = (IDictionary<string, object>)record;
            string Text(string field) => fields.TryGetValue(field, out var value) ? value?.ToString()?.Trim() ?? "" : "";
            var name = Text("Nombre");
            if (!existing.TryGetValue(MonsterCsvParsing.NormalizeName(name), out var spell))
            {
                spell = new SpellOfTerror(0, name, Text("Descripción"), Text("Tipo"), TypeAppearsParser.Parse(Text("Apariciones")));
                _context.SpellsOfTerror.Add(spell);
                existing[MonsterCsvParsing.NormalizeName(name)] = spell;
                continue;
            }

            spell.Name = name;
            spell.Description = Text("Descripción");
            spell.Type = Text("Tipo");
            spell.Appears = TypeAppearsParser.Parse(Text("Apariciones"));
        }

        await _context.SaveChangesAsync();
    }

    private async Task ImportMonsterSpellLinksAsync(string monsterCsvPath)
    {
        var monstersByName = (await _context.Monsters.AsNoTracking().ToListAsync())
            .ToDictionary(monster => MonsterCsvParsing.NormalizeName(monster.Name));
        var spellsByName = (await _context.SpellsOfTerror.AsNoTracking().ToListAsync())
            .GroupBy(spell => MonsterCsvParsing.NormalizeName(spell.Name))
            .ToDictionary(group => group.Key, group => group.First());
        var links = new HashSet<(int MonsterId, int SpellId)>();

        using var reader = new StreamReader(monsterCsvPath);
        using var csv = new CsvReader(reader, GetConfig());
        foreach (var record in csv.GetRecords<dynamic>())
        {
            var fields = (IDictionary<string, object>)record;
            string Text(string field) => fields.TryGetValue(field, out var value) ? value?.ToString()?.Trim() ?? "" : "";
            if (!monstersByName.TryGetValue(MonsterCsvParsing.NormalizeName(Text("Nombre")), out var monster)) continue;

            foreach (var abilityName in MonsterCsvParsing.ReadAbilityNames(Text("Habilidad")))
            {
                if (spellsByName.TryGetValue(MonsterCsvParsing.NormalizeName(abilityName), out var spell))
                    links.Add((monster.Id, spell.Id));
            }
        }

        // Sólo los nombres que también existen en SpellsOfTerror producen una relación con clave foránea.
        _context.MonsterSpellOfTerrors.AddRange(links.Select(link => new MonsterSpellOfTerror
        {
            MonsterId = link.MonsterId,
            SpellOfTerrorId = link.SpellId
        }));
        await _context.SaveChangesAsync();
    }

    private async Task ImportMiscellaneaAsync(string ruta)
    {
        if (!File.Exists(ruta)) return;
        if (await _context.Miscellanea.AnyAsync()) return;

        using var reader = new StreamReader(ruta);
        using var csv = new CsvReader(reader, GetConfig());
        var registros = csv.GetRecords<dynamic>().ToList();

        foreach (var fila in registros)
        {
            var datos = (IDictionary<string, object>)fila;
            string Texto(string campo) => datos.ContainsKey(campo) ? datos[campo]?.ToString()?.Trim() ?? "" : "";
            int Numero(string campo) => int.TryParse(Texto(campo), out var valor) ? valor : 0;

            var entry = new MiscellaneaItem(
                id: 0,
                name: Texto("Nombre"),
                description: Texto("Descripción"),
                price: Numero("Precio (Monedas de Oro)"),
                appears: TypeAppearsParser.Parse(Texto("Apariciones"))
            );
            _context.Miscellanea.Add(entry);
        }
        await _context.SaveChangesAsync();
    }

    private async Task ImportEquipmentsAndArtifactsAsync(string equipmentPath, string artifactPath)
    {
        if (await _context.EquipmentsAndArtifacts.AnyAsync()) return;

        if (File.Exists(equipmentPath))
        {
            using var reader = new StreamReader(equipmentPath);
            using var csv = new CsvReader(reader, GetConfig());
            var registros = csv.GetRecords<dynamic>().ToList();

            foreach (var fila in registros)
            {
                var datos = (IDictionary<string, object>)fila;
                string Texto(string campo) => datos.ContainsKey(campo) ? datos[campo]?.ToString()?.Trim() ?? "" : "";
                int Numero(string campo) => int.TryParse(Texto(campo), out var valor) ? valor : 0;

                var item = new EquipmentsAndArtifacts(
                    id: 0,
                    name: Texto("Nombre"),
                    description: Texto("Descripción"),
                    price: Numero("Precio (Monedas de Oro)"),
                    type: Texto("Tipo"),
                    appears: TypeAppearsParser.Parse(Texto("Apariciones")),
                    weight: Texto("Peso"),
                    category: "Equipment"
                );
                _context.EquipmentsAndArtifacts.Add(item);
            }
        }

        if (File.Exists(artifactPath))
        {
            using var reader = new StreamReader(artifactPath);
            using var csv = new CsvReader(reader, GetConfig());
            var registros = csv.GetRecords<dynamic>().ToList();

            foreach (var fila in registros)
            {
                var datos = (IDictionary<string, object>)fila;
                string Texto(string campo) => datos.ContainsKey(campo) ? datos[campo]?.ToString()?.Trim() ?? "" : "";

                var item = new EquipmentsAndArtifacts(
                    id: 0,
                    name: Texto("Nombre"),
                    description: Texto("Descripción"),
                    price: 0,
                    type: Texto("Tipo"),
                    appears: TypeAppearsParser.Parse(Texto("Apariciones")),
                    weight: "",
                    category: "Artifact"
                );
                _context.EquipmentsAndArtifacts.Add(item);
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task ImportMonsterAbilitiesAsync(string ruta)
    {
        if (!File.Exists(ruta)) return;
        if (await _context.MonsterAbilities.AnyAsync()) return;

        using var reader = new StreamReader(ruta);
        using var csv = new CsvReader(reader, GetConfig());
        var registros = csv.GetRecords<dynamic>().ToList();

        foreach (var fila in registros)
        {
            var datos = (IDictionary<string, object>)fila;
            string Texto(string campo) => datos.ContainsKey(campo) ? datos[campo]?.ToString()?.Trim() ?? "" : "";

            var ability = new MonsterAbility(
                id: 0,
                name: Texto("Nombre"),
                description: Texto("Descripción"),
                appears: TypeAppearsParser.Parse(Texto("Apariciones"))
            );
            _context.MonsterAbilities.Add(ability);
        }
        await _context.SaveChangesAsync();
    }
}
