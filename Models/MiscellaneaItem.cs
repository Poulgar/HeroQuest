namespace HeroQuest.Models;

public class MiscellaneaItem(int id, string name, string description, int price, TypeAppears appears) : IArchivable
{
    // Entidad de la sección Miscelánea: conserva el mismo esquema de datos de la antigua tabla Items.
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public int Price { get; set; } = price;
    public TypeAppears Appears { get; set; } = appears;
    public MiscellaneaCategory Category { get; set; } = MiscellaneaCategory.NormalShop;
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
    public bool IsArchived { get; set; }
}
