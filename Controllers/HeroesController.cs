using HeroQuest.Data;
using HeroQuest.Models;
using HeroQuest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Controllers;

public class HeroesController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index(int? id)
    {
        var heroes = await context.Heroes.AsNoTracking()
            .OrderBy(hero => hero.Name)
            .ThenBy(hero => hero.Id)
            .ToListAsync();

        var selected = heroes.FirstOrDefault(hero => hero.Id == id) ?? heroes.FirstOrDefault();
        ViewData["Title"] = "Héroes";

        return View(new HeroCatalogViewModel { Heroes = heroes, SelectedHero = selected });
    }
}
