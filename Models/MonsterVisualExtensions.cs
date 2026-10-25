namespace HeroQuest.Models;

public static class MonsterVisualExtensions
{
    public static string ShortEffect(this string? description, string name, int maxLength = 120)
    {
        // Estos resúmenes describen el efecto en una frase; el texto completo sigue en la ficha emergente al pulsar.
        var summary = name.Trim() switch
        {
            "Ágil" => "Ignora los efectos de terrenos peligrosos, muebles y héroes al moverse.",
            "Ataque a Distancia" => "Puede atacar a objetivos no adyacentes que estén en su línea de visión.",
            "Ataque de Abrazo" => "Atrapa al héroe y le inflige daño cada turno hasta que escape o caiga el monstruo.",
            "Ataque Doble" => "Realiza dos ataques por turno, contra uno o dos oponentes.",
            "Ataque en Diagonal" => "Puede atacar también en diagonal.",
            "Engendrar" => "Puede crear o mover engendros al final del turno de cada héroe.",
            "Estratega Astuto" => "Puede moverse antes y después de actuar, y golpea con más fuerza si un aliado rodea al objetivo.",
            "Muerto Viviente" => "Carece de mente y es inmune a efectos dirigidos a la mente.",
            "Raíces Apresadoras" => "Detiene el movimiento de los héroes que entran en una casilla adyacente.",
            "Robar Objetos" => "Puede robar un objeto permitido a un héroe y huir con él.",
            "Último Aliento" => "Una vez, al llegar a cero puntos corporales, se queda con uno.",
            "Venenoso" => "Su veneno puede paralizar a la víctima y limitar su movimiento.",
            "Vuelo Dracónico" => "Puede intercalar una acción durante su movimiento y atravesar casillas ocupadas.",
            "Agarre Trepador" => "Enreda a un héroe con lianas hasta que él u otro héroe las destruya.",
            "Aliento Ácido" => "Daña a todos los presentes en una habitación, excepto al lanzador.",
            "Bola en Llamas" => "Envuelve a un héroe en llamas y le causa daño corporal.",
            "Calma" => "Restaura hasta tres puntos corporales al lanzador o a un monstruo.",
            "Canalizar Terror" => "Debilita a un héroe y puede hacerle perder puntos corporales.",
            "Congelación Mental" => "Puede reducir a cero la mente de un héroe y dejarlo en shock.",
            "Contra hechizo" => "Intenta cancelar un hechizo que acaba de lanzar un héroe.",
            "Dominación" => "Pone a un héroe bajo el control de Zargon hasta que rompa el hechizo.",
            "Dormir" => "Impide que un héroe afectado se mueva, ataque o se defienda.",
            "Escalofrío" => "Inflige daño a una víctima adyacente, que no puede defenderse.",
            "Espejo Mágico" => "Devuelve al héroe un hechizo que este acaba de lanzar.",
            "Estallido Mental" or "Explosión Mental" => "Paraliza a un héroe visible e impide que actúe.",
            "Huída Fugaz" => "Permite al lanzador escapar rápidamente del peligro.",
            "Invocar Espectros" => "Llama espectros para que acudan al combate.",
            "Invocar Lobos" => "Convoca lobos que se unen al combate.",
            "Invocar Muertos Vivientes" => "Reanima muertos vivientes para luchar bajo su mando.",
            "Invocar Orcos" => "Convoca orcos para reforzar el combate.",
            "La Maldición del Hombre Lobo" => "Somete a la víctima a una maldición que la transforma en hombre lobo.",
            "Luces del Terror" => "Desorienta y atemoriza a los héroes con luces sobrenaturales.",
            "Miedo" => "Debilita el valor de un héroe y puede hacerle perder puntos corporales.",
            "Muro de Hielo" => "Levanta una barrera de hielo que bloquea el paso.",
            "Nube de Terror" => "Envuelve una zona en una nube que causa terror.",
            "Oxidación" => "Corroe y deteriora el equipo de su víctima.",
            "Parálisis Mental" => "Bloquea la mente de un héroe y puede impedir que actúe.",
            "Patinar" => "Hace que la víctima resbale y se desplace sobre el hielo.",
            "Rayo Mortífero" => "Lanza un rayo mágico capaz de infligir daño grave.",
            "Reanimación" => "Devuelve a un muerto viviente a la actividad.",
            "Restaurar Terror" => "Recupera fuerzas para un monstruo ligado al Terror.",
            "Subyugación" => "Somete a un héroe a la voluntad de Zargon.",
            "Tempestad" => "Desata una tormenta que entorpece a quienes quedan atrapados.",
            "Tormenta de Fuego" => "Desata fuego mágico sobre sus víctimas.",
            "Tormenta de Hielo" => "Desata una tormenta helada contra sus víctimas.",
            _ => null
        };

        if (summary is not null) return summary;
        if (string.IsNullOrWhiteSpace(description)) return "Sin descripción registrada.";
        var clean = string.Join(' ', description.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        var sentenceEnd = clean.IndexOfAny(['.', '!', '?']);
        if (sentenceEnd > 0 && sentenceEnd < maxLength) clean = clean[..(sentenceEnd + 1)];
        if (clean.Length <= maxLength) return clean;

        // El fallback también corta al final de una palabra para nombres nuevos de CSV.
        var cutAt = clean.LastIndexOf(' ', maxLength - 1);
        return clean[..(cutAt > 0 ? cutAt : maxLength)].TrimEnd() + "…";
    }

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

    public static string SpellSymbol(this string spell) => spell.Trim() switch
    {
        "Agarre Trepador" => "♧",
        "Aliento Ácido" => "♨",
        "Bola en Llamas" or "Tormenta de Fuego" => "✹",
        "Calma" or "Restaurar Terror" => "☼",
        "Canalizar Terror" or "Miedo" or "Nube de Terror" => "☠",
        "Congelación Mental" or "Escalofrío" or "Muro de Hielo" or "Tormenta de Hielo" or "Patinar" => "❄",
        "Contra hechizo" or "Espejo Mágico" => "⛨",
        "Dominación" or "Subyugación" => "♜",
        "Dormir" => "☾",
        "Estallido Mental" or "Explosión Mental" => "✷",
        "Huída Fugaz" => "➶",
        "Invocar Espectros" or "Invocar Muertos Vivientes" => "♰",
        "Invocar Lobos" => "♧",
        "Invocar Orcos" => "⚒",
        "La Maldición del Hombre Lobo" => "☽",
        "Oxidación" => "⚒",
        "Parálisis Mental" or "Rayo Mortífero" => "↯",
        _ => "✦"
    };

    public static string SpellColor(this string spell)
    {
        // El mismo algoritmo determinista diferencia cromáticamente cada hechizo en tarjetas e iconos.
        var palette = new[] { "#8b3f35", "#346d82", "#687b39", "#875f29", "#714d83", "#3e7863", "#a04f68", "#a35a2b", "#48589a" };
        uint hash = 2166136261;
        foreach (var character in spell.Trim().ToUpperInvariant())
            hash = unchecked((hash ^ character) * 16777619);
        return palette[(int)(hash % (uint)palette.Length)];
    }
}
