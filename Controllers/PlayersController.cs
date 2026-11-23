using System.Globalization;
using System.Text;
using HeroQuest.Data;
using HeroQuest.Models;
using HeroQuest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Controllers;

/// <summary>El Maestro de Juego crea y mantiene las cuentas y sus héroes activos.</summary>
[Authorize(Roles = "Admin")]
public class PlayersController(ApplicationDbContext db, HeroConfigurationService configurations) : Controller
{
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlayerProfileInput input)
    {
        var heroes = await GetValidHeroesAsync(input.HeroIds);
        if (!ModelState.IsValid || heroes is null)
        {
            TempData["PlayerFormError"] = "Revisa los datos y elige únicamente héroes activos.";
            return RedirectToAction("Index", "Home");
        }

        var username = CreateUsername(input.Name);
        if (await db.PlayerProfiles.AnyAsync(player => player.Username == username)
            || UserNameBelongsToDemoAccount(username))
        {
            TempData["PlayerFormError"] = $"El nombre de acceso «{username}» ya está ocupado.";
            return RedirectToAction("Index", "Home");
        }

        var profile = new PlayerProfile
        {
            Name = input.Name.Trim(), Nickname = string.IsNullOrWhiteSpace(input.Nickname) ? null : input.Nickname.Trim(), Username = username,
            PasswordHash = PasswordHashing.Create(username + "123")
        };
        db.PlayerProfiles.Add(profile);
        foreach (var hero in heroes) await configurations.CreateAsync(profile, hero);
        await db.SaveChangesAsync();

        TempData["PlayerFormMessage"] = "El jugador y sus configuraciones de héroe ya están registrados.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var profile = await db.PlayerProfiles.Include(player => player.HeroConfigurations).FirstOrDefaultAsync(player => player.Id == id);
        if (profile is null) return NotFound();
        return View(new PlayerProfileEditViewModel
        {
            Id = profile.Id, Name = profile.Name, Nickname = profile.Nickname ?? string.Empty, Username = profile.Username,
            SelectedHeroIds = profile.HeroConfigurations.Where(configuration => configuration.IsActive).Select(configuration => configuration.HeroId).ToList(),
            Heroes = await db.Heroes.AsNoTracking().Where(hero => !hero.IsArchived).OrderBy(hero => hero.CharacterTypeClass).ToListAsync()
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PlayerProfileEditInput input)
    {
        var profile = await db.PlayerProfiles.Include(player => player.HeroConfigurations).FirstOrDefaultAsync(player => player.Id == input.Id);
        var heroes = await GetValidHeroesAsync(input.HeroIds);
        if (profile is null) return NotFound();
        if (!ModelState.IsValid || heroes is null)
        {
            TempData["PlayerFormError"] = "Revisa los datos y vuelve a elegir los héroes activos.";
            return RedirectToAction(nameof(Edit), new { id = input.Id });
        }

        profile.Name = input.Name.Trim();
        profile.Nickname = string.IsNullOrWhiteSpace(input.Nickname) ? null : input.Nickname.Trim();
        var selectedIds = heroes.Select(hero => hero.Id).ToHashSet();
        foreach (var configuration in profile.HeroConfigurations) configuration.IsActive = selectedIds.Contains(configuration.HeroId);
        foreach (var hero in heroes) await configurations.CreateAsync(profile, hero);
        await db.SaveChangesAsync();
        TempData["PlayerFormMessage"] = "Los datos del jugador y sus héroes activos se han actualizado.";
        return RedirectToAction("Index", "Home");
    }

    private async Task<List<Heroe>?> GetValidHeroesAsync(IEnumerable<int> ids)
    {
        var selected = ids.Distinct().ToArray();
        var heroes = await db.Heroes.Where(hero => selected.Contains(hero.Id) && !hero.IsArchived).ToListAsync();
        return heroes.Count == selected.Length ? heroes : null;
    }

    private bool UserNameBelongsToDemoAccount(string username)
        => configurationNames().Any(name => name.Equals(username, StringComparison.OrdinalIgnoreCase));

    private IEnumerable<string> configurationNames()
        => HttpContext.RequestServices.GetRequiredService<IConfiguration>().GetSection("DemoAccounts").GetChildren()
            .Select(account => account["Username"] ?? string.Empty);

    private static string CreateUsername(string playerName)
    {
        var decomposed = playerName.Trim().Normalize(NormalizationForm.FormD);
        var plain = new string(decomposed.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
        if (plain.Length == 0) plain = "aventurero";
        return plain.Length > 60 ? plain[..60] : plain;
    }
}
