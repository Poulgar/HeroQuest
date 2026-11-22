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
        // Include carga las tablas relacionadas de hechizos para que la ficha no dispare consultas adicionales.
        var monsters = await context.Monsters.AsNoTracking().Where(monster => !monster.IsArchived)
            .Include(monster => monster.SpellLinks)
                .ThenInclude(link => link.SpellOfTerror)
            .OrderBy(monster => monster.Id)
            .ToListAsync();
        var abilities = await context.MonsterAbilities.AsNoTracking().Where(ability => !ability.IsArchived).OrderBy(ability => ability.Id).ToListAsync();
        var spells = await context.SpellsOfTerror.AsNoTracking().Where(spell => !spell.IsArchived).OrderBy(spell => spell.Id).ToListAsync();

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
