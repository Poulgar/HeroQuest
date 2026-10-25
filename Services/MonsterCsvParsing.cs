using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace HeroQuest.Services;

internal static class MonsterCsvParsing
{
    private static readonly Regex LinkedAbilityPattern = new(
        @"(?<name>[^()]+?)\s*\(https?://[^)]+\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static IReadOnlyList<string> ReadAbilityNames(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return [];

        // Cada nombre de Notion precede a su URL; capturamos ese nombre y descartamos el enlace técnico.
        var names = LinkedAbilityPattern.Matches(value)
            .Select(match => match.Groups["name"].Value.Trim())
            .Where(name => name.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return names.Count > 0
            ? names
            : value.Split([';', '|'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }

    public static string CleanAbilityNames(string? value)
        => string.Join(", ", ReadAbilityNames(value));

    public static string NormalizeName(string value)
    {
        // Quitamos tildes para emparejar nombres aunque un CSV o una URL los escriba de forma distinta.
        var decomposed = value.Trim().Normalize(NormalizationForm.FormD);
        var withoutAccents = decomposed.Where(character =>
            CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark);
        return string.Concat(withoutAccents).Normalize(NormalizationForm.FormC).ToUpperInvariant();
    }
}
