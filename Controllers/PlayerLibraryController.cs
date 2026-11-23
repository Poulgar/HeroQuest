using System.Security.Claims;
using HeroQuest.Data;
using HeroQuest.Models;
using HeroQuest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Controllers;

/// <summary>Permite al Aventurero consultar y editar sólo sus configuraciones de héroe.</summary>
[Authorize(Roles = "User")]
public class PlayerLibraryController(ApplicationDbContext db, HeroConfigurationService service, WorldPoemService poems) : Controller
{
    public async Task<IActionResult> Index()
    {
        if (!TryGetProfileId(out var profileId)) return Forbid();
        var configurations = await db.HeroConfigurations.AsNoTracking()
            .Where(configuration => configuration.PlayerProfileId == profileId && configuration.IsActive)
            .Include(configuration => configuration.Hero).OrderBy(configuration => configuration.Hero.CharacterTypeClass).ToListAsync();
        return View(new PlayerHeroListViewModel { Configurations = configurations });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!TryGetProfileId(out var profileId)) return Forbid();
        var configuration = await db.HeroConfigurations.Include(item => item.Hero)
            .FirstOrDefaultAsync(item => item.Id == id && item.PlayerProfileId == profileId && item.IsActive);
        if (configuration is null) return NotFound();
        var model = await service.BuildEditorAsync(configuration.Hero, configuration, personal: true);
        await PopulateOptionsAsync(configuration.Hero, model);
        return View("~/Views/Admin/Edit.cshtml", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateField(int id, string field, string? value, string[]? values)
    {
        if (!TryGetProfileId(out var profileId)) return Forbid();
        var configuration = await db.HeroConfigurations.Include(item => item.Hero)
            .FirstOrDefaultAsync(item => item.Id == id && item.PlayerProfileId == profileId && item.IsActive);
        if (configuration is null) return NotFound();

        // El servicio valida los nombres y restricciones de equipo; ningún dato de la ficha maestra se acepta aquí.
        var error = await service.UpdateAsync(configuration, field, value, values ?? []);
        if (error is not null) return BadRequest(error);
        var editor = await service.BuildEditorAsync(configuration.Hero, configuration, personal: true);
        return Json(new
        {
            success = true,
            message = await poems.MessageAsync("library-edited", configuration.CharacterName),
            stats = new { hp = editor.DisplayHp, mind = editor.DisplayMind, attack = editor.DisplayAttack, defense = editor.DisplayDefense, movement = editor.DicesRedMovement },
            marks = editor.ComparisonMarks
        });
    }

    private async Task PopulateOptionsAsync(Heroe hero, CatalogEditViewModel model)
    {
        ViewBag.WeaponOptions = (await service.GetWeaponOptionsAsync(hero)).Concat(model.WeaponOptions).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(name => name).ToArray();
        ViewBag.ArmorOptions = (await service.GetArmorOptionsAsync(hero)).Concat(model.ArmorOptions).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(name => name).ToArray();
        ViewBag.ItemOptions = (await service.GetItemOptionsAsync(hero)).Concat(model.ItemOptions).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(name => name).ToArray();
        var abilities = await db.MonsterAbilities.AsNoTracking().Where(item => !item.IsArchived).Select(item => item.Name)
            .Concat(db.SpellsOfTerror.AsNoTracking().Where(item => !item.IsArchived).Select(item => item.Name)).ToListAsync();
        ViewBag.AbilityCatalogOptions = abilities.Concat(model.AbilityOptions).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(name => name).ToArray();
    }

    private bool TryGetProfileId(out int profileId)
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out profileId);
}
