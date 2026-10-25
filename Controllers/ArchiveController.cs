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
        var items = await context.EquipmentsAndArtifacts.AsNoTracking()
            .OrderBy(e => e.Category)
            .ThenBy(e => e.Type)
            .ThenBy(e => e.Name)
            .ToListAsync();

        var selected = items.FirstOrDefault(e => e.Id == id) ?? items.FirstOrDefault();
        ViewData["Title"] = "Equipo y artefactos";

        return View("~/Views/EquipmentsArtifacts/Index.cshtml", new EquipmentsArtifactsViewModel { Items = items, SelectedItem = selected });
    }

    public async Task<IActionResult> Modifiers(int? id)
    {
        var items = await context.Items.AsNoTracking()
            .OrderBy(i => i.Name)
            .ToListAsync();

        var selected = items.FirstOrDefault(i => i.Id == id) ?? items.FirstOrDefault();
        ViewData["Title"] = "Modificadores";

        return View("~/Views/Modifiers/Index.cshtml", new ItemsViewModel { Items = items, SelectedItem = selected });
    }
}
