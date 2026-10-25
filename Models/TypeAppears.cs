using System.ComponentModel.DataAnnotations;

namespace HeroQuest.Models;

public enum TypeAppears
{
    [Display (Name = "Cripta de la Oscuridad")]
    CriptaDeLaOscuridad,
    [Display (Name = "Despertar")]
    Despertar,
    [Display (Name = "Horda del Ogro")]
    HordaDelOgro,
    [Display (Name = "Horror Congelado")]
    HorrorCongelado,
    [Display (Name = "Juego Base")]
    JuegoBase,
    [Display (Name = "Junglas de Delthrak")]
    JunglasDeDelthrak,
    [Display (Name = "Luna del Terror")]
    LunaDelTerror,
    [Display (Name = "Maga del Espejo")]
    MagaDelEspejo,
    [Display (Name = "Profecía de Telor")]
    ProfeciaDeTelor,
    [Display (Name = "Reina de los Espíritus")]
    ReinaDeLosEspiritus,
    [Display (Name = "Señor de los Brujos")]
    SeñorDeLosBrujos,
    [Display (Name = "Torre de Kellar")]
    TorreDeKellar,

    [Display (Name = "Original")]
    Original
}