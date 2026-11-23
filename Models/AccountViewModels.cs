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
    public int? ConfigurationId { get; set; }
    public bool IsPersonalConfiguration { get; set; }
    public List<HeroConfigurationTab> ConfigurationTabs { get; set; } = [];
    public Dictionary<string, string> ComparisonMarks { get; set; } = [];
    public int DisplayHp { get; set; }
    public int DisplayMind { get; set; }
    public int DisplayAttack { get; set; }
    public int DisplayDefense { get; set; }
    public string? SourceName { get; set; }
    public string Key { get; set; } = string.Empty;
    [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
    [StringLength(2000)] public string Description { get; set; } = string.Empty;
    public string CharacterTypeClass { get; set; } = string.Empty;
    public string Faction { get; set; } = string.Empty;
    public string Ability { get; set; } = string.Empty;
    public string[] AbilityOptions { get; set; } = [];
    public string Category { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
    public string SpellType { get; set; } = string.Empty;
    public string[] AppearsOptions { get; set; } = ["Original"];
    public int Hp { get; set; }
    public int DicesMind { get; set; }
    public int DicesAttack { get; set; }
    public int DicesDefend { get; set; }
    public int DicesRedMovement { get; set; }
    public int MaxMovement { get; set; }
    public int Price { get; set; }
    public string[] MiscellaneaCategoryOptions { get; set; } = [MiscellaneaCategory.NormalShop.DisplayName()];
    public int Gold { get; set; }
    public bool IsArchived { get; set; }
    public string Weapon { get; set; } = string.Empty;
    public string Armor { get; set; } = string.Empty;
    public string[] WeaponOptions { get; set; } = [];
    public string[] ArmorOptions { get; set; } = [];
    public string[] ItemOptions { get; set; } = [];
    public string Spells { get; set; } = string.Empty;
    public string Items { get; set; } = string.Empty;
    public int QuestsCompleted { get; set; }
}

public sealed record HeroConfigurationTab(int Id, string PlayerName, string CharacterName, bool IsActive);

public sealed record CatalogListItem(
    int Id, string Name, string Details, bool IsArchived,
    string SearchText, string Expansion, string Classification);
public sealed class CatalogListViewModel
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string CreateLabel { get; set; } = "Añadir ficha";
    public List<CatalogListItem> Items { get; set; } = [];
    public List<CatalogListItem> ArchivedItems { get; set; } = [];
    public List<string> ExpansionOptions { get; set; } = [];
    public List<string> ClassificationOptions { get; set; } = [];
    public string ClassificationLabel { get; set; } = "Categoría / clase";
}

public sealed class LibraryListSectionViewModel
{
    public List<CatalogListItem> Items { get; set; } = [];
    public string Section { get; set; } = string.Empty;
    public CatalogListViewModel Library { get; set; } = null!;
}

public sealed record SearchPickerViewModel(
    string FieldName, string Label, IReadOnlyList<string> Options,
    IReadOnlyList<string> SelectedValues, bool AllowMultiple, bool Locked);

public sealed class PlayerProfileInput
{
    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.StringLength(60)]
    public string Nickname { get; set; } = string.Empty;

    public int[] HeroIds { get; set; } = [];
}

public sealed class PlayerProfileEditInput
{
    public int Id { get; set; }
    [Required, StringLength(80)] public string Name { get; set; } = string.Empty;
    [StringLength(60)] public string Nickname { get; set; } = string.Empty;
    public int[] HeroIds { get; set; } = [];
}

public sealed class PlayerProfileEditViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public List<int> SelectedHeroIds { get; set; } = [];
    public List<Heroe> Heroes { get; set; } = [];
}

public sealed class PlayerHeroListViewModel
{
    public List<HeroConfiguration> Configurations { get; set; } = [];
}
