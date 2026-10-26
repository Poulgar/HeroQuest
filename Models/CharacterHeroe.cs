namespace HeroQuest.Models;

public class Heroe : Character
{
    public Heroe(int id, string characterTypeClass, int hp, int dicesMind, int dicesAttack, int dicesDefend, int dicesRedMovement, string description)
        : base(id, characterTypeClass, hp, dicesMind, dicesAttack, dicesDefend, description)
    {
        DicesRedMovement = dicesRedMovement;
    }

    public Heroe() : base(0, string.Empty, 0, 0, 0, 0, string.Empty)
    {
    }

    public int DicesRedMovement { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Player { get; set; } = string.Empty;
    public string Weapon { get; set; } = string.Empty;
    public string? Armor { get; set; }
    public string? Spells { get; set; }
    public int Gold { get; set; }
    public string? Items { get; set; }
    public int QuestsCompleted { get; set; }
}
