using CsvHelper;
using CsvHelper.Configuration;
using HeroQuest.Models;
using System.Globalization;
using HeroQuest.Data;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Services;

public class HeroeImportService
{
    private readonly ApplicationDbContext _context;

    public HeroeImportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ImportarAsync(string ruta) // Esta línea define un método asincrónico para importar datos de héroes desde un archivo CSV.
    {
        using var reader = new StreamReader(ruta); // Crea un lector de archivos para leer el contenido del archivo CSV especificado por la ruta proporcionada.

        if (await _context.Heroes.AnyAsync()) // Verifica si ya existen registros de héroes en la base de datos. Si hay al menos un registro, se asume que los datos ya han sido importados y no se realiza ninguna acción adicional.
        {
            return;
        }

        var config = new CsvConfiguration(
            CultureInfo.InvariantCulture) // Crea una configuración para el lector de CSV, especificando que se utilizará la cultura invariante para el análisis de los datos.
        {
            Delimiter = ",",
            PrepareHeaderForMatch = args =>
                args.Header.Trim()
        };

        using var csv = new CsvReader(reader, config); // Crea un lector de CSV utilizando el lector de archivos y la configuración especificada. Este lector se utilizará para leer y analizar los registros del archivo CSV.

        var registros = csv.GetRecords<dynamic>().ToList(); // Obtiene todos los registros del archivo CSV como una lista de objetos dinámicos. Cada registro representa una fila en el archivo CSV y se almacena en la lista "registros".

        foreach (var fila in registros)
        {
            var datos = (IDictionary<string, object>)fila; // Convierte cada fila del archivo CSV en un diccionario de pares clave-valor, donde las claves son los nombres de las columnas y los valores son los datos correspondientes a cada columna.

            string Texto(string campo) =>
                datos[campo]?.ToString()?.Trim() ?? ""; // Esta función auxiliar toma el nombre de un campo (columna) como parámetro y devuelve el valor correspondiente de la fila actual como una cadena de texto. Si el valor es nulo, se devuelve una cadena vacía. Además, se eliminan los espacios en blanco al principio y al final del valor.

            int Numero(string campo) =>
                int.TryParse(Texto(campo), out var valor)
                    ? valor
                    : 0; // Esta función auxiliar toma el nombre de un campo (columna) como parámetro y devuelve el valor correspondiente de la fila actual como un número entero. Si el valor no se puede convertir a un número entero, se devuelve 0.

            var heroe = new Heroe(
                id: 0,
                characterType: Texto("CharacterType"),
                hp: Numero("Hp"),
                mind: Numero("Mind"),
                dicesAttack: Numero("DicesAttack"),
                dicesDefend: Numero("DicesDefend"),
                dicesMovement: Numero("DicesMovement"),
                description: Texto("Description")) // Crea una nueva instancia de la clase Heroe utilizando los valores obtenidos de la fila actual del archivo CSV. Se pasan los valores correspondientes a las propiedades del héroe, como el tipo de personaje, puntos de vida, mente, dados de ataque y dados de defensa.
            {
                CharacterType = Texto("CharacterType"),
                Name = Texto("Name"),
                Player = Texto("Player"),
                Weapon = Texto("Weapon"),
                Armor = Texto("Armor"),
                Spells = Texto("Spells"),
                Gold = Numero("Gold"),
                Items = Texto("Items"),
                QuestsCompleted = Numero("QuestsCompleted")
            };

            _context.Set<Heroe>().Add(heroe); // Agrega la instancia del héroe recién creada al contexto de la base de datos, marcándola para su inserción en la tabla correspondiente cuando se guarden los cambios en la base de datos.
        }

        await _context.SaveChangesAsync(); // Guarda los cambios realizados en el contexto de la base de datos, lo que implica insertar todos los héroes importados en la tabla correspondiente de la base de datos. Esta operación se realiza de manera asincrónica para no bloquear el hilo principal.
    }
}
