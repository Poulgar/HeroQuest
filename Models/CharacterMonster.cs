using System.ComponentModel;

namespace HeroQuest.Models;

public class Monster : Character
{
    public Monster(int id, string characterType, int hp, int mind, int dicesAttack, int dicesDefend, int maxMovement)
        : base(id, characterType, hp, mind, dicesAttack, dicesDefend, description: string.Empty)
    {
        MaxMovement = maxMovement;
    }

    public Monster(int id, string characterType, int hp, int mind, int dicesAttack, int dicesDefend, int maxMovement, string name, int movement, int attack, int defense, int body, string ability, string appears)
        : base(id, characterType, hp, mind, dicesAttack, dicesDefend, description: string.Empty)
    {
        MaxMovement = maxMovement;
        Name = name;
        Movement = movement;
        Attack = attack;
        Defense = defense;
        Body = body;
        Ability = ability;
    }

    public Monster() : base(0, string.Empty, 0, 0, 0, 0, string.Empty)
    {
    }

    public int MaxMovement { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Movement { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Body { get; set; }
    public string Ability { get; set; } = string.Empty;
    [DefaultValue(TypeMonsterRol.Regular)]
    public TypeMonsterRol TypeMonsterRol { get; set; }
}
