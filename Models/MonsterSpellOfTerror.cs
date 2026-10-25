namespace HeroQuest.Models;

/// <summary>Tabla puente: un monstruo puede tener varios hechizos y cada hechizo puede pertenecer a varios monstruos.</summary>
public class MonsterSpellOfTerror
{
    public int MonsterId { get; set; }
    public Monster Monster { get; set; } = null!;
    public int SpellOfTerrorId { get; set; }
    public SpellOfTerror SpellOfTerror { get; set; } = null!;
}
