using HeroQuest.Data;
using HeroQuest.Models;
using HeroQuest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Controllers;

public class MonstersController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index(int? id)
    {
        var monsters = await context.Monsters.AsNoTracking()
            .OrderBy(monster => monster.CharacterType)
            .ThenBy(monster => monster.Id)
            .ToListAsync();
        var abilities = await context.MonsterAbilities.AsNoTracking()
            .OrderBy(ability => ability.Name)
            .ToListAsync();

        var selected = monsters.FirstOrDefault(monster => monster.Id == id) ?? monsters.FirstOrDefault();
        ViewData["Title"] = "Monstruos";

        return View("MonstersIndex", new MonsterCatalogViewModel
        {
            Monsters = monsters,
            SelectedMonster = selected,
            Abilities = abilities
        });
    }
}
