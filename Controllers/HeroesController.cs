using HeroQuest.Data;
using HeroQuest.Models;
using HeroQuest.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Controllers;

public class HeroesController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index(int? id)
    {
        var query = context.Heroes.AsNoTracking().Where(hero => !hero.IsArchived);
        // Un Aventurero solo ve en este índice las clases que tiene vinculadas actualmente.
        if (User.IsInRole("User"))
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var profileId)) return Forbid();
            var activeHeroIds = context.HeroConfigurations.Where(configuration => configuration.PlayerProfileId == profileId && configuration.IsActive)
                .Select(configuration => configuration.HeroId);
            query = query.Where(hero => activeHeroIds.Contains(hero.Id));
        }
        var heroes = await query.OrderBy(hero => hero.Id).ToListAsync();

        var selected = heroes.FirstOrDefault(hero => hero.Id == id) ?? heroes.FirstOrDefault();
        ViewData["Title"] = "Héroes";

        return View("HeroesIndex", new HeroCatalogViewModel { Heroes = heroes, SelectedHero = selected });
    }
}
