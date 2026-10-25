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
        var monstersPath = Path.Combine(csvFolderPath, "Monstruos (1) 3241d9a0efd682bc9ec1816c829fd228.csv");
        var itemsPath = Path.Combine(csvFolderPath, "Objetos (1) 8311d9a0efd682478309012935880a6d.csv");
        var equipmentPath = Path.Combine(csvFolderPath, "Equipo (1) 3731d9a0efd6822e9d4a01c22d86ba7d.csv");
        var artifactsPath = Path.Combine(csvFolderPath, "Artefactos (1) 04b1d9a0efd683b59ae20122de2c154c.csv");
        var abilitiesPath = Path.Combine(csvFolderPath, "Habilidades de Monstruos (1) 7f31d9a0efd68269889801464866b7dc.csv");

        // No ocultamos una ruta incorrecta: así la app informa por qué no pudo llenar una tabla.
        foreach (var path in new[] { monstersPath, itemsPath, equipmentPath, artifactsPath, abilitiesPath })
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"No se encontró el CSV requerido: {Path.GetFileName(path)}", path);
        }

        await ImportMonstersAsync(monstersPath);
        await ImportItemsAsync(itemsPath); // Objetos/Modificadores
        await ImportEquipmentsAndArtifactsAsync(
            equipmentPath, // Equipamiento
            artifactsPath); // Artefactos
        await ImportMonsterAbilitiesAsync(abilitiesPath); // Habilidades
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
        if (await _context.Monsters.AnyAsync()) return;

        // Heroes y Monsters comparten la clave de Character aunque se guarden en tablas distintas (TPC).
        // Empezamos después del ID más alto de las dos tablas para evitar colisiones al rastrear entidades.
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
            var movement = Number("Movimiento");
            var attack = Number("Ataque");
            var defense = Number("Defensa");
            var body = Number("Cuerpo");

            // El modelo conserva estadísticas comunes en Character y las etiquetas del bestiario en Monster.
            // Asignamos ambas representaciones desde las mismas columnas para que las consultas y la vista coincidan.
            var monster = new Monster(nextId++, name, body, Number("Mente"), attack, defense, movement)
            {
                Name = name,
                Movement = movement,
                Attack = attack,
                Defense = defense,
                Body = body,
                Ability = Text("Habilidad"),
                Appears = TypeAppearsParser.Parse(Text("Apariciones"))
            };

            _context.Monsters.Add(monster);
        }

        await _context.SaveChangesAsync();
    }

    private async Task ImportItemsAsync(string ruta)
    {
        if (!File.Exists(ruta)) return;
        if (await _context.Items.AnyAsync()) return;

        using var reader = new StreamReader(ruta);
        using var csv = new CsvReader(reader, GetConfig());
        var registros = csv.GetRecords<dynamic>().ToList();

        foreach (var fila in registros)
        {
            var datos = (IDictionary<string, object>)fila;
            string Texto(string campo) => datos.ContainsKey(campo) ? datos[campo]?.ToString()?.Trim() ?? "" : "";
            int Numero(string campo) => int.TryParse(Texto(campo), out var valor) ? valor : 0;

            var item = new Item(
                id: 0,
                name: Texto("Nombre"),
                description: Texto("Descripción"),
                price: Numero("Precio (Monedas de Oro)"),
                appears: TypeAppearsParser.Parse(Texto("Apariciones"))
            );
            _context.Items.Add(item);
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
