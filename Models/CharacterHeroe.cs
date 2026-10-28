namespace HeroQuest.Models;

public class Heroe : Character
{
    public Heroe(int id, string characterTypeClass, int hp, int dicesMind, int dicesAttack, int dicesDefend, int dicesRedMovement, string description)
        : base(id, characterTypeClass, hp, dicesMind, dicesAttack, dicesDefend, description)
    {
        DicesRedMovement = dicesRedMovement; // El constructor solo se ejecuta cuando se invoque con <<new Heroe(...)>> por lo que técnicamente la propiedad DicesRedMovement ya esta declarada de antes, por eso puede ser inicializada.
    }

    public Heroe() : base(0, string.Empty, 0, 0, 0, 0, string.Empty) // Los valores de Character se pasan como "nulos" para evitar errores, en lugar de pasarlos por sus nombres.
    {
    }

    public int DicesRedMovement { get; set; } // Declaración de DicesRedMovement.
    public string Name { get; set; } = string.Empty;
    public string Player { get; set; } = string.Empty;
    public string Weapon { get; set; } = string.Empty;
    public string? Armor { get; set; }
    public string? Spells { get; set; }
    public int Gold { get; set; }
    public string? Items { get; set; }
    public int QuestsCompleted { get; set; }
}
