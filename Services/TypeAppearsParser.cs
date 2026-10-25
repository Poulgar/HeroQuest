using HeroQuest.Models;

namespace HeroQuest.Services;

/// <summary>
/// Convierte todas las expansiones de una celda CSV en las banderas combinables de TypeAppears.
/// </summary>
internal static class TypeAppearsParser
{
    public static TypeAppears Parse(string? appearances)
    {
        if (string.IsNullOrWhiteSpace(appearances))
            return TypeAppears.Original;

        // Los CSV usan normalmente comas; también aceptamos punto y coma y barra vertical como separadores.
        var parsed = TypeAppears.None;
        foreach (var token in appearances.Split([',', ';', '|'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            parsed |= token switch
            {
                "Cripta de la Oscuridad" => TypeAppears.CriptaDeLaOscuridad,
                "Despertar" => TypeAppears.Despertar,
                "Horda del Ogro" => TypeAppears.HordaDelOgro,
                "Horror Congelado" => TypeAppears.HorrorCongelado,
                "Juego Base" => TypeAppears.JuegoBase,
                "Junglas de Delthrak" => TypeAppears.JunglasDeDelthrak,
                "Luna del Terror" => TypeAppears.LunaDelTerror,
                "Maga del Espejo" => TypeAppears.MagaDelEspejo,
                "Profecía de Telor" => TypeAppears.ProfeciaDeTelor,
                "Reina de los Espíritus" => TypeAppears.ReinaDeLosEspiritus,
                "Señor de los Brujos" => TypeAppears.SeñorDeLosBrujos,
                "Torre de Kellar" => TypeAppears.TorreDeKellar,
                "Original" or "Not In Game" => TypeAppears.Original,
                _ => TypeAppears.None
            };
        }

        // Si no reconocimos ningún nombre, mantenemos el comportamiento anterior y lo clasificamos como Original.
        return parsed == TypeAppears.None ? TypeAppears.Original : parsed;
    }
}
