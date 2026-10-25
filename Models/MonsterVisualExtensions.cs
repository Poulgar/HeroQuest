namespace HeroQuest.Models;

public static class MonsterVisualExtensions
{
    public static string FactionSymbol(this string faction) => faction.Trim() switch
    {
        "Bestias" => "♧",
        "Criaturas Congeladas" => "❄",
        "Criaturas del Terror" => "☠",
        "Elfos" => "➶",
        "Muertos Vivientes" => "♰",
        "Ogros" => "♟",
        "Pieles Verdes" => "⚒",
        "Singulares" => "✦",
        _ => "⚜"
    };

    public static string FactionColor(this string faction) => faction.Trim() switch
    {
        "Bestias" => "#54753c",
        "Criaturas Congeladas" => "#297d9c",
        "Criaturas del Terror" => "#7d2d38",
        "Elfos" => "#567b43",
        "Muertos Vivientes" => "#555d53",
        "Ogros" => "#8a5d31",
        "Pieles Verdes" => "#46743b",
        "Singulares" => "#78529a",
        _ => "#806536"
    };

    public static string AbilitySymbol(this string ability) => ability.Trim() switch
    {
        "Ágil" => "↯",
        "Ataque a Distancia" => "➶",
        "Ataque de Abrazo" => "♧",
        "Ataque Doble" => "⚔",
        "Ataque en Diagonal" => "⤢",
        "Engendrar" => "✹",
        "Estratega Astuto" => "♜",
        "Muerto Viviente" => "☠",
        "Raíces Apresadoras" => "❧",
        "Robar Objetos" => "◇",
        "Último Aliento" => "♨",
        "Venenoso" => "☣",
        "Vuelo Dracónico" => "♢",
        _ => "✦"
    };

    public static string AbilityColor(this string ability)
    {
        // El checksum estable da a cada habilidad un color repetible incluso tras reiniciar la aplicación.
        var palette = new[] { "#8b3f35", "#346d82", "#687b39", "#875f29", "#714d83", "#3e7863", "#a04f68" };
        uint hash = 2166136261;
        foreach (var character in ability.Trim().ToUpperInvariant())
            hash = unchecked((hash ^ character) * 16777619);
        return palette[(int)(hash % (uint)palette.Length)];
    }
}
