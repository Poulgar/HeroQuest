using System.ComponentModel.DataAnnotations;

namespace HeroQuest.Models;

public sealed class LoginViewModel
{
    [Required, Display(Name = "Nombre de viajero")]
    public string Username { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Display(Name = "Código secreto")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>Campos comunes que permiten administrar los distintos catálogos desde un formulario reutilizable.</summary>
public sealed class CatalogEditViewModel
{
    public string Type { get; set; } = string.Empty;
    public int? Id { get; set; }
    public string Key { get; set; } = string.Empty;
    [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
    [StringLength(2000)] public string Description { get; set; } = string.Empty;
    public string CharacterTypeClass { get; set; } = string.Empty;
    public string Faction { get; set; } = string.Empty;
    public string Ability { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
    public string SpellType { get; set; } = string.Empty;
    public string Appears { get; set; } = "Original";
    public int Hp { get; set; }
    public int DicesMind { get; set; }
    public int DicesAttack { get; set; }
    public int DicesDefend { get; set; }
    public int DicesRedMovement { get; set; }
    public int MaxMovement { get; set; }
    public int Price { get; set; }
    public int Gold { get; set; }
    public bool IsArchived { get; set; }
    public string Player { get; set; } = string.Empty;
    public string Weapon { get; set; } = string.Empty;
    public string Armor { get; set; } = string.Empty;
    public string Spells { get; set; } = string.Empty;
    public string Items { get; set; } = string.Empty;
    public int QuestsCompleted { get; set; }
}

public sealed record CatalogListItem(int Id, string Name, string Details, bool IsArchived);
public sealed class CatalogListViewModel
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<CatalogListItem> Items { get; set; } = [];
}
