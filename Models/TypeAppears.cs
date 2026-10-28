using System.ComponentModel.DataAnnotations;

namespace HeroQuest.Models;

[Flags] // Indica que un mismo elemento TypeAppears puede tener varios valores a la vez.
public enum TypeAppears
{
    // 1 << n significa que el valor es 2 elevado a la potencia de n, lo que permite representar cada valor como un bit único en un número binario.
    [Display(Name = "Sin especificar")]
    None = 0, // Binario: 000000000000, indica que el elemento no aparece en ninguna expansión.
    [Display (Name = "Cripta de la Oscuridad")]
    CriptaDeLaOscuridad = 1 << 0, // Binario: 000000000001.
    [Display (Name = "Despertar")]
    Despertar = 1 << 1, // Binario: 000000000010.
    [Display (Name = "Horda del Ogro")]
    HordaDelOgro = 1 << 2, // Binario: 000000000100.
    [Display (Name = "Horror Congelado")]
    HorrorCongelado = 1 << 3, // Binario: 000000001000.
    [Display (Name = "Juego Base")]
    JuegoBase = 1 << 4, // Binario: 000000010000.
    [Display (Name = "Junglas de Delthrak")]
    JunglasDeDelthrak = 1 << 5, // Binario: 000000100000.
    [Display (Name = "Luna del Terror")]
    LunaDelTerror = 1 << 6, // Binario: 000001000000.
    [Display (Name = "Maga del Espejo")]
    MagaDelEspejo = 1 << 7, // Binario: 000010000000.
    [Display (Name = "Profecía de Telor")]
    ProfeciaDeTelor = 1 << 8, // Binario: 000100000000.
    [Display (Name = "Reina de los Espíritus")]
    ReinaDeLosEspiritus = 1 << 9, // Binario: 001000000000.
    [Display (Name = "Señor de los Brujos")]
    SeñorDeLosBrujos = 1 << 10, // Binario: 010000000000.
    [Display (Name = "Torre de Kellar")]
    TorreDeKellar = 1 << 11, // Binario: 100000000000.

    [Display (Name = "Original")]
    Original = 1 << 12

    /*EJEMPLO
    CharacterHeroe: Bárbaro
    Expansiones: Horror Congelado (dec 8 - bin 000000001000) , Juego Base (dec 16 - bin 000000010000)
    Guardado en la BBDD como: (dec 24 - bin 000000011000)*/
}
