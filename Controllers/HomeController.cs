using System.Diagnostics;
using HeroQuest.Data;
using HeroQuest.Models;
using HeroQuest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Controllers;

public class HomeController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        // La portada solo necesita recuentos; evitamos cargar todas las entidades en memoria.
        var equipment = context.EquipmentsAndArtifacts.AsNoTracking();
        var model = new HeroQuestIndexViewModel
        {
            HeroesCount = await context.Heroes.CountAsync(item => !item.IsArchived),
            MonstersCount = await context.Monsters.CountAsync(item => !item.IsArchived),
            EquipmentCount = await equipment.CountAsync(item => !item.IsArchived && item.Category == "Equipment"),
            ArtifactCount = await equipment.CountAsync(item => !item.IsArchived && item.Category == "Artifact"),
            MiscellaneaCount = await context.Miscellanea.CountAsync(item => !item.IsArchived),
            MonsterAbilityCount = await context.MonsterAbilities.CountAsync(item => !item.IsArchived),
            SpellCount = await context.SpellsOfTerror.CountAsync(item => !item.IsArchived)
        };

        // Los datos privados del grupo solo se cargan para la sesión del Maestro de Juego.
        if (User.IsInRole("Admin"))
        {
            model.Players = await context.PlayerProfiles.AsNoTracking().Include(player => player.Hero)
                .OrderBy(player => player.Name).ToListAsync();
            model.AvailableHeroes = await context.Heroes.AsNoTracking().Where(hero => !hero.IsArchived)
                .OrderBy(hero => hero.Name).ToListAsync();
        }

        return View("HomeIndex", model);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    });
}
