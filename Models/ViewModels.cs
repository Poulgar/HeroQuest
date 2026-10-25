using HeroQuest.Models;

namespace HeroQuest.ViewModels;

public class HeroQuestIndexViewModel
{
    public int HeroesCount { get; set; }
    public int MonstersCount { get; set; }
    public int EquipmentCount { get; set; }
    public int MiscellaneaCount { get; set; }
    public int MonsterAbilityCount { get; set; }
}

public class HeroCatalogViewModel
{
    public List<Heroe> Heroes { get; set; } = [];
    public Heroe? SelectedHero { get; set; }
}

public class MonsterCatalogViewModel
{
    public List<Monster> Monsters { get; set; } = [];
    public Monster? SelectedMonster { get; set; }
    // El modelo aún no relaciona una habilidad con un monstruo concreto; se muestra el catálogo completo.
    public List<MonsterAbility> Abilities { get; set; } = [];
}

public class EquipmentsArtifactsViewModel
{
    public List<EquipmentsAndArtifacts> Items { get; set; } = [];
    public EquipmentsAndArtifacts? SelectedItem { get; set; }
}

public class MiscellaneaViewModel
{
    public List<MiscellaneaItem> Entries { get; set; } = [];
    public MiscellaneaItem? SelectedEntry { get; set; }
}
