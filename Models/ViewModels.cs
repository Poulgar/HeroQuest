using HeroQuest.Models;

namespace HeroQuest.ViewModels;

public class HeroQuestIndexViewModel
{
    public List<Heroe> Heroes { get; set; } = [];
    public List<Monster> Monsters { get; set; } = [];
}