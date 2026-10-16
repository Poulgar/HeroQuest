using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using HeroQuest.Data;
using HeroQuest.Models;
using HeroQuest.ViewModels;

namespace HeroQuest.Controllers;

public class HomeController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var viewModel = new HeroQuestIndexViewModel
        {
            Heroes = await context.Heroes.AsNoTracking().OrderBy(h => h.Id).ToListAsync(),
            Monsters = await context.Monsters.AsNoTracking().OrderBy(m => m.CharacterType).ToListAsync()
        };

        return View(viewModel);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}