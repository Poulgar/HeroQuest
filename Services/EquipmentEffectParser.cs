using System.Text.RegularExpressions;
using HeroQuest.Models;

namespace HeroQuest.Services;

/// <summary>Extrae modificadores explícitos de las descripciones españolas de equipo y objetos.</summary>
public static partial class EquipmentEffectParser
{
    private static readonly Regex AttackDicePattern = new(@"(?:(?<n>\d+)\s+Dados?\s+de\s+(?:Ataque|Combate)|Fuerza\s+de\s+(?:Ataque|Combate)(?:\s+de)?\s+(?<n2>\d+)\s+Dados?)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex DefensePattern = new(@"(?:(?:otorga|da|añade|proporciona|tira)\s+)?(?<n>\d+)\s+Dados?\s+de\s+Defensa\s+adicional(?:es)?", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex HealthPattern = new(@"(?:(?:añade|otorga|da|resta|reduce)\s+)(?<n>\d+)\s+Puntos?\s+Corporales?", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex MindPattern = new(@"(?:(?:añade|otorga|da|resta|reduce)\s+)(?<n>\d+)\s+Puntos?\s+Mentales?", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static void Apply(EquipmentsAndArtifacts item)
    {
        var effect = Parse(item.Name, item.Description, item.Type);
        item.WeaponAttackDice = effect.Attack;
        item.DefenseBonus = effect.Defense;
        item.HealthBonus = effect.Health;
        item.MindBonus = effect.Mind;
        item.MovementDiceOverride = effect.MovementOverride;
        item.MovementSquaresBonus = effect.MovementSquares;
        item.AllowedClasses = effect.Allowed;
        item.ExcludedClasses = effect.Excluded;
        item.EquipmentSlot = effect.Slot;
        item.CannotCombineWith = effect.CannotCombine;
        item.IsRangedWeapon = effect.IsRanged;
    }

    public static void Apply(MiscellaneaItem item)
    {
        var effect = Parse(item.Name, item.Description, "Objeto");
        item.WeaponAttackDice = effect.Attack;
        item.DefenseBonus = effect.Defense;
        item.HealthBonus = effect.Health;
        item.MindBonus = effect.Mind;
        item.MovementDiceOverride = effect.MovementOverride;
        item.MovementSquaresBonus = effect.MovementSquares;
        item.AllowedClasses = effect.Allowed;
        item.ExcludedClasses = effect.Excluded;
        item.EquipmentSlot = effect.Slot;
        item.CannotCombineWith = effect.CannotCombine;
        item.IsRangedWeapon = effect.IsRanged;
    }

    private static ParsedEffect Parse(string name, string description, string type)
    {
        var attack = FirstNumber(AttackDicePattern, description);
        var defense = FirstNumber(DefensePattern, description);
        var health = SignedNumber(HealthPattern, description);
        var mind = SignedNumber(MindPattern, description);
        var movementOverride = Regex.IsMatch(description, @"solo puedes (?:tirar|lanzar) 1 Dado Rojo de Movimiento", RegexOptions.IgnoreCase) ? 1 : (int?)null;
        var movementSquares = Regex.Match(description, @"(?:añadir|añade)\s+(?<n>\d+)\s+casillas?\s+(?:a|de)\s+(?:tu\s+)?(?:tirada\s+de\s+)?movimiento", RegexOptions.IgnoreCase);
        var allowed = Regex.Match(description, @"Solo puede (?:ser utilizado|ser utilizada|utilizarlo|llevarla|llevarlo) por (?:el|la) ([\p{L} ]+?)(?:[.,\r\n]|$)", RegexOptions.IgnoreCase);
        var excluded = Regex.Matches(description, @"No puede ser (?:utilizado|utilizada) por (?:el|la) ([\p{L} ]+?)(?:[.,\r\n]|$)", RegexOptions.IgnoreCase)
            .Select(match => NormalizeClass(match.Groups[1].Value)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var ranged = type.Equals("Arma", StringComparison.OrdinalIgnoreCase)
            && Regex.IsMatch(description, @"largo alcance|disparar|(?:puede )?lanzarse a cualquier Monstruo que puedas ver|puede ser lanzada", RegexOptions.IgnoreCase);
        var slot = type.Equals("Arma", StringComparison.OrdinalIgnoreCase) ? "Arma"
            : name.Contains("Escudo", StringComparison.OrdinalIgnoreCase) ? "Escudo"
            : name.Contains("Yelmo", StringComparison.OrdinalIgnoreCase) || name.Contains("Casco", StringComparison.OrdinalIgnoreCase) ? "Cabeza"
            : type.Equals("Armadura", StringComparison.OrdinalIgnoreCase) ? "Armadura" : "Objeto";
        // Las incompatibilidades se leen de la tarjeta de cada pieza y luego se validan entre los objetos equipados.
        var incompatibleItems = Regex.Matches(description, @"(?:no puede ser combinada con|no puedes utilizar|ni con)\s*(?:un|una|el|la)?\s*(Escudo|Hacha de Batalla|Bast[oó]n)", RegexOptions.IgnoreCase)
            .Select(match => match.Groups[1].Value).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var cannotCombine = string.Join("|", incompatibleItems);

        return new ParsedEffect(attack, defense, health, mind, movementOverride,
            movementSquares.Success ? ParseNumber(movementSquares.Groups["n"].Value) : 0,
            allowed.Success ? NormalizeClass(allowed.Groups[1].Value) : string.Empty,
            string.Join("|", excluded), slot, cannotCombine, ranged);
    }

    private static int FirstNumber(Regex pattern, string text)
    {
        var match = pattern.Match(text);
        if (!match.Success) return 0;
        var group = match.Groups["n"].Success ? match.Groups["n"] : match.Groups["n2"];
        return ParseNumber(group.Value);
    }

    private static int SignedNumber(Regex pattern, string text)
    {
        var match = pattern.Match(text);
        if (!match.Success) return 0;
        var number = ParseNumber(match.Groups["n"].Value);
        return Regex.IsMatch(match.Value, @"resta|reduce", RegexOptions.IgnoreCase) ? -number : number;
    }

    private static int ParseNumber(string value) => int.TryParse(value, out var number) ? number : 0;
    private static string NormalizeClass(string value) => value.Trim().TrimEnd('.', ',', ';');

    private sealed record ParsedEffect(int Attack, int Defense, int Health, int Mind, int? MovementOverride,
        int MovementSquares, string Allowed, string Excluded, string Slot, string CannotCombine, bool IsRanged);
}
