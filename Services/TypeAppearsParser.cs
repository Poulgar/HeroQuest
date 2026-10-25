using HeroQuest.Models;

namespace HeroQuest.Services;

/// <summary>
/// Convierte el texto de expansión de los CSV al enum que guarda el modelo.
/// El modelo admite una sola expansión, por eso se conserva la primera si el CSV enumera varias.
/// </summary>
internal static class TypeAppearsParser
{
    public static TypeAppears Parse(string? appearances)
    {
        if (string.IsNullOrWhiteSpace(appearances))
            return TypeAppears.Original;

        var firstAppearance = appearances.Split(',', 2)[0].Trim();

        return firstAppearance switch
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
            _ => TypeAppears.Original
        };
    }
}
