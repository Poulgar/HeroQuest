using System.Diagnostics;
using HeroQuest.Data;
using HeroQuest.Models;
using HeroQuest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Controllers;

public class HomeController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index() // Se define como async para poder ejecutar todas las instrucciones en simultaneo optimizando el programa.
    {
        // La portada solo necesita recuentos; evitamos cargar todas las entidades en memoria.
        var equipment = context.EquipmentsAndArtifacts.AsNoTracking(); // <<.AsNoTracking()>> lo único que hace es evitar que EF Core guarde una copia interna de los datos y ralentice el programa. <<Task>> es una promesa, indica que el método asincrono se terminará más adelante y enviará los resultados en cuanto esté listo.
        var model = new HeroQuestIndexViewModel
        {
            HeroesCount = await context.Heroes.CountAsync(),
            MonstersCount = await context.Monsters.CountAsync(),
            EquipmentCount = await equipment.CountAsync(item => item.Category == "Equipment"),
            ArtifactCount = await equipment.CountAsync(item => item.Category == "Artifact"),
            MiscellaneaCount = await context.Miscellanea.CountAsync(),
            MonsterAbilityCount = await context.MonsterAbilities.CountAsync(),
            SpellCount = await context.SpellsOfTerror.CountAsync()
        };

        return View("HomeIndex", model); // Busca el archivo HomeIndex.cshtml y envia model.
    }

    public IActionResult Privacy() => View();

    // Muestra la página de error cuando falla la aplicación.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Indica que no debe de almacenarla en cache ni en ningún otro lugar.
    public IActionResult Error() => View(new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    });
}
