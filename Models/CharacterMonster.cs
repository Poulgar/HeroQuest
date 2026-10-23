using System.ComponentModel;

namespace HeroQuest.Models;

public class Monster(int id, string characterType, int hp, int mind, int dicesAttack, int dicesDefend, int maxMovement) : Character(id, characterType, hp, mind, dicesAttack, dicesDefend, description: string.Empty)
{
    public int MaxMovement { get; set; } = maxMovement;

    [DefaultValue(TypeMonsterRol.Regular)]
    public TypeMonsterRol TypeMonsterRol { get; set; }
}