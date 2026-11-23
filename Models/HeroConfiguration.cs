namespace HeroQuest.Models;

/// <summary>Estado de un héroe para una persona y partida; la ficha Heroe sigue siendo la referencia inmutable.</summary>
public class HeroConfiguration
{
    public int Id { get; set; }
    public int HeroId { get; set; }
    public Heroe Hero { get; set; } = null!;
    public int PlayerProfileId { get; set; }
    public PlayerProfile PlayerProfile { get; set; } = null!;
    public string CharacterName { get; set; } = string.Empty;

    // Nombres delimitados por | para soportar varias piezas sin duplicar tablas de enlace en esta fase.
    public string Weapons { get; set; } = string.Empty;
    public string Armors { get; set; } = string.Empty;
    public string Items { get; set; } = string.Empty;
    public string Abilities { get; set; } = string.Empty;
    public int MovementDice { get; set; }
    public int Gold { get; set; }
    public int QuestsCompleted { get; set; }
    public bool IsActive { get; set; } = true;
}
