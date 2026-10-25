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
        var model = new HeroQuestIndexViewModel
        {
            HeroesCount = await context.Heroes.CountAsync(),
            MonstersCount = await context.Monsters.CountAsync(),
            EquipmentCount = await context.EquipmentsAndArtifacts.CountAsync(),
            ModifierCount = await context.Items.CountAsync(),
            MonsterAbilityCount = await context.MonsterAbilities.CountAsync()
        };

        return View("HomeIndex", model);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    });
}
