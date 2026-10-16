namespace HeroQuest.Models;

public class Heroe(int id, string characterType, int hp, int mind, int dicesAttack, int dicesDefend, int dicesMovement, string description) : Character(id, characterType, hp, mind, dicesAttack, dicesDefend, dicesMovement, description)
{
    public string Name { get; set; } = string.Empty;
    public string Player { get; set; } = string.Empty;
    public required string Weapon { get; set; }
    public string? Armor { get; set; }
    public string? Spells { get; set; }
    public int Gold { get; set; }
    public string? Items { get; set; }
    public int QuestsCompleted { get; set; }
}