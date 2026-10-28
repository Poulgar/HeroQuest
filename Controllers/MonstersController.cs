using HeroQuest.Data;
using HeroQuest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Controllers;

public class MonstersController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index(int? id)
    {
        // Include carga las tablas relacionadas de hechizos para que la ficha no dispare consultas adicionales.
        var monsters = await context.Monsters.AsNoTracking()
            .Include(monster => monster.SpellLinks)
                .ThenInclude(link => link.SpellOfTerror)
            .OrderBy(monster => monster.Id)
            .ToListAsync();
        var abilities = await context.MonsterAbilities.AsNoTracking().OrderBy(ability => ability.Id).ToListAsync();
        var spells = await context.SpellsOfTerror.AsNoTracking().OrderBy(spell => spell.Id).ToListAsync();

        var selected = monsters.FirstOrDefault(monster => monster.Id == id) ?? monsters.FirstOrDefault();
        ViewData["Title"] = "Monstruos";

        return View("MonstersIndex", new MonsterCatalogViewModel
        {
            Monsters = monsters,
            SelectedMonster = selected,
            Abilities = abilities,
            SpellsOfTerror = spells
        });
    }
}
