using System.ComponentModel.DataAnnotations;

namespace HeroQuest.Models;

/// <summary>Procedencias y usos de un objeto; son combinables porque una poción puede pertenecer a varias categorías.</summary>
[Flags]
public enum MiscellaneaCategory
{
    [Display(Name = "Tienda normal")] NormalShop = 1,
    [Display(Name = "Mercado negro")] BlackMarket = 2,
    [Display(Name = "Tesoro")] Treasure = 4,
    [Display(Name = "Alquimia")] Alchemy = 8,
    [Display(Name = "Alquimia elaborada")] CraftedAlchemy = 16,
    [Display(Name = "Exclusivo del Elfo")] ElfExclusive = 32,
    [Display(Name = "Exclusivo del Bárbaro")] BarbarianExclusive = 64
}
