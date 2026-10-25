namespace HeroQuest.Models;

public class Character
{
    public Character(int id, string characterType, int hp, int mind, int dicesAttack, int dicesDefend, string description)
    {
        Id = id;
        CharacterType = characterType;
        Hp = hp;
        HpCurrent = hp;
        Mind = mind;
        DicesAttack = dicesAttack;
        DicesDefend = dicesDefend;
        Description = description;
    }

    public Character()
    {
    }

    public int Id { get; set; }
    public string CharacterType { get; set; } = string.Empty;
    public int Hp { get; set; }
    public int HpCurrent { get; set; }
    public int Mind { get; set; }
    public int DicesAttack { get; set; }
    public int DicesDefend { get; set; }
    public string? Description { get; set; }
    public TypeAppears Appears { get; set; }
}