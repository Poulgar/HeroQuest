namespace HeroQuest.Models;

public class Monster : Character
{
    public Monster(int id, string name, int hp, int dicesMind, int dicesAttack, int dicesDefend, int maxMovement, string ability, string faction, string description)
        : base(id, name, hp, dicesMind, dicesAttack, dicesDefend, description)
    {
        Name = name;
        MaxMovement = maxMovement;
        Ability = ability;
        Faction = faction;
        Attack = dicesAttack;
        Defense = dicesDefend;
    }

    public Monster() : base(0, string.Empty, 0, 0, 0, 0, string.Empty)
    {
    }

    public int MaxMovement { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Attack { get; set; }
    public int Defense { get; set; }
    public string Ability { get; set; } = string.Empty;
    public string Faction { get; set; } = string.Empty;
    public ICollection<MonsterSpellOfTerror> SpellLinks { get; set; } = [];

    /*List<T> se usa para el código interno.
    ICollection<T> se usa para las Interfaces*/
}
