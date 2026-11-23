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
        // Esta vista es un catálogo de referencia; nunca carga ni edita las configuraciones personales.
        var heroes = await context.Heroes.AsNoTracking().Where(hero => !hero.IsArchived).OrderBy(hero => hero.Id).ToListAsync();

        var selected = heroes.FirstOrDefault(hero => hero.Id == id) ?? heroes.FirstOrDefault();
        ViewData["Title"] = "Héroes";

        return View("HeroesIndex", new HeroCatalogViewModel { Heroes = heroes, SelectedHero = selected });
    }
}
