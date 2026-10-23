using System.ComponentModel.DataAnnotations;

namespace HeroQuest.Models;

public enum TypeMonsterRol
{
    [Display (Name = "Regular")]
    Regular,
    [Display (Name = "Jefe")]
    Boss,
    [Display (Name = "Original")]
    Original
}