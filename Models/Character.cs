namespace HeroQuest.Models;

public class Character(int id, string characterType, int hp, int mind, int dicesAttack, int dicesDefend, int dicesMovement, string description) // A partir de C# 12 las propiedades se declaran en el Primary Constructor y posteriormente se asignan.
{
    public int Id { get; set; } = id;
    public required string CharacterType { get; set; } = characterType;
    public int Hp { get; set; } = hp;
    public int HpCurrent { get; set; } = hp;
    public int Mind { get; set; } = mind;
    public int DicesAttack { get; set; } = dicesAttack;
    public int DicesDefend { get; set; } = dicesDefend;
    public int DicesMovement { get; set; } = dicesMovement;
    public string? Description { get; set; } = description;
}