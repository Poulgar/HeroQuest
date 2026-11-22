namespace HeroQuest.Models;

public class Character : IArchivable
{
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

    public Character()
    {
    }

    public int Id { get; set; }
    // Nombre de origen que permite recuperar la ficha de referencia aunque el título visible se edite.
    public string? SourceName { get; set; }
    public string CharacterTypeClass { get; set; } = string.Empty;
    public int Hp { get; set; }
    public int DicesMind { get; set; }
    public int DicesAttack { get; set; }
    public int DicesDefend { get; set; }
    public string? Description { get; set; }
    public TypeAppears Appears { get; set; }

    // Archivar oculta una ficha en el catálogo sin borrarla ni romper referencias de la base de datos.
    public bool IsArchived { get; set; }
}
