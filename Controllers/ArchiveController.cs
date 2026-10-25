using HeroQuest.Data;
using HeroQuest.Models;
using HeroQuest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Controllers;

public class ArchiveController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Equipment(int? id)
    {
        var items = await context.EquipmentsAndArtifacts.AsNoTracking().OrderBy(e => e.Id).ToListAsync();

        var selected = items.FirstOrDefault(e => e.Id == id) ?? items.FirstOrDefault();
        ViewData["Title"] = "Equipo y artefactos";

        return View("~/Views/EquipmentsArtifacts/Index.cshtml", new EquipmentsArtifactsViewModel { Items = items, SelectedItem = selected });
    }

    public async Task<IActionResult> Miscellanea(int? id)
    {
        // Ordenamos por ID porque AlphabeticalIdService mantiene esos IDs alineados con el nombre.
        var entries = await context.Miscellanea.AsNoTracking().OrderBy(entry => entry.Id).ToListAsync();

        var selectedEntry = entries.FirstOrDefault(entry => entry.Id == id) ?? entries.FirstOrDefault();
        ViewData["Title"] = "Miscelánea";

        return View("~/Views/Miscellanea/Index.cshtml", new MiscellaneaViewModel { Entries = entries, SelectedEntry = selectedEntry });
    }
}
