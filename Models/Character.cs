namespace HeroQuest.Models;

public class Character
{
    // Constructor con parametros para crear un Character a partir de su estructura.
    public Character(int id, string characterTypeClass, int hp, int dicesMind, int dicesAttack, int dicesDefend, string description)
    {
        Id = id;
        CharacterTypeClass = characterTypeClass;
        Hp = hp;
        DicesMind = dicesMind;
        DicesAttack = dicesAttack;
        DicesDefend = dicesDefend;
        Description = description;
    }

    // Constructor sin parametros para crear un Character sin seguir un orden especifico.
    public Character()
    {
    }

    // Campos de la clase Character.
    public int Id { get; set; }
    public string CharacterTypeClass { get; set; } = string.Empty;
    public int Hp { get; set; }
    public int DicesMind { get; set; }
    public int DicesAttack { get; set; }
    public int DicesDefend { get; set; }
    public string? Description { get; set; }
    public TypeAppears Appears { get; set; }
}
