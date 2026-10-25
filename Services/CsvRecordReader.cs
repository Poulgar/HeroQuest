using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace HeroQuest.Services;

/// <summary>Centraliza la lectura segura y la conversión básica compartida por los importadores CSV.</summary>
internal static class CsvRecordReader
{
    public static List<IDictionary<string, object>> ReadRecords(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CreateConfig());
        // Materializamos las filas antes de cerrar el lector para entregar datos independientes del archivo.
        return csv.GetRecords<dynamic>().Cast<IDictionary<string, object>>().ToList();
    }

    public static string GetText(IDictionary<string, object> fields, string name)
        => fields.TryGetValue(name, out var value) ? value?.ToString()?.Trim() ?? string.Empty : string.Empty;

    public static int GetInteger(IDictionary<string, object> fields, string name)
        => int.TryParse(GetText(fields, name), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : 0;

    private static CsvConfiguration CreateConfig() => new(CultureInfo.InvariantCulture)
    {
        Delimiter = ",",
        PrepareHeaderForMatch = args => args.Header.Trim(),
        BadDataFound = null,
        MissingFieldFound = null
    };
}
