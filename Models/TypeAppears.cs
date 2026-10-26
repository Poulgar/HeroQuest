using System.ComponentModel.DataAnnotations;

namespace HeroQuest.Models;

[Flags]
public enum TypeAppears
{
    [Display(Name = "Sin especificar")]
    None = 0,
    [Display (Name = "Cripta de la Oscuridad")]
    CriptaDeLaOscuridad = 1 << 0,
    [Display (Name = "Despertar")]
    Despertar = 1 << 1,
    [Display (Name = "Horda del Ogro")]
    HordaDelOgro = 1 << 2,
    [Display (Name = "Horror Congelado")]
    HorrorCongelado = 1 << 3,
    [Display (Name = "Juego Base")]
    JuegoBase = 1 << 4,
    [Display (Name = "Junglas de Delthrak")]
    JunglasDeDelthrak = 1 << 5,
    [Display (Name = "Luna del Terror")]
    LunaDelTerror = 1 << 6,
    [Display (Name = "Maga del Espejo")]
    MagaDelEspejo = 1 << 7,
    [Display (Name = "Profecía de Telor")]
    ProfeciaDeTelor = 1 << 8,
    [Display (Name = "Reina de los Espíritus")]
    ReinaDeLosEspiritus = 1 << 9,
    [Display (Name = "Señor de los Brujos")]
    SeñorDeLosBrujos = 1 << 10,
    [Display (Name = "Torre de Kellar")]
    TorreDeKellar = 1 << 11,

    [Display (Name = "Original")]
    Original = 1 << 12
}
