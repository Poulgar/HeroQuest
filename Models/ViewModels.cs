using HeroQuest.Models;

namespace HeroQuest.ViewModels;

public class HeroQuestIndexViewModel
{
    public List<Heroe> Heroes { get; set; } = [];
    public List<Monster> Monsters { get; set; } = [];
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
}
