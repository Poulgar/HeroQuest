namespace HeroQuest.Models;

public class EquipmentsAndArtifacts(int id, string name, string description, int price, string type, TypeAppears appears, string weight, string category) : IArchivable
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public int Price { get; set; } = price;
    public string Type { get; set; } = type;
    public TypeAppears Appears { get; set; } = appears;
    public string Weight { get; set; } = weight;
    public string Category { get; set; } = category; // "Equipment" or "Artifact"
    public bool IsArchived { get; set; }
    public int? WeaponAttackDice { get; set; }
    public int DefenseBonus { get; set; }
    public int HealthBonus { get; set; }
    public int MindBonus { get; set; }
    public int? MovementDiceOverride { get; set; }
    public int MovementSquaresBonus { get; set; }
    public string AllowedClasses { get; set; } = string.Empty;
    public string ExcludedClasses { get; set; } = string.Empty;
    public string EquipmentSlot { get; set; } = string.Empty;
    public string CannotCombineWith { get; set; } = string.Empty;
    public bool IsRangedWeapon { get; set; }
}
