namespace HeroQuest.Models;

/// <summary>Tabla puente: un monstruo puede tener varios hechizos y cada hechizo puede pertenecer a varios monstruos.</summary>
public class MonsterSpellOfTerror
{
    public int MonsterId { get; set; }
    public Monster Monster { get; set; } = null!; // <<= null!;>> indica que va a tener valor aunque se inicialice sin uno.
    public int SpellOfTerrorId { get; set; }
    public SpellOfTerror SpellOfTerror { get; set; } = null!;

    /*1. Propiedad opcional: puede ser null.
    El compilador obliga a comprobarla antes de usarla.
    public Monster? Monster { get; set; }

    2. Propiedad no opcional según el diseño, inicializada como null.
    El "!" suprime el aviso del compilador; EF la rellenará después.
    public Monster Monster { get; set; } = null!;

    3. Propiedad no opcional con un objeto real desde el inicio.
    Nunca empieza siendo null.
    public Monster Monster { get; set; } = new Monster();*/
}
