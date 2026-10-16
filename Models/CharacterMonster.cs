namespace HeroQuest.Models;

public class Monster : Character
{
    public Monster(int id, string characterType, int hp, int mind, int dicesAttack, int dicesDefend, int dicesMovement)
        : base(id, characterType, hp, mind, dicesAttack, dicesDefend, dicesMovement, description: string.Empty)
    {
    }
}