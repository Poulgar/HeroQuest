using HeroQuest.Data;
using HeroQuest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Controllers;

/// <summary>Gestiona los datos básicos de los jugadores del grupo, solo desde la cuenta del Maestro de Juego.</summary>
[Authorize(Roles = "Admin")]
public class PlayersController(ApplicationDbContext db) : Controller
{
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlayerProfileInput input)
    {
        if (!ModelState.IsValid || (input.HeroId is int heroId &&
            (!await db.Heroes.AnyAsync(hero => hero.Id == heroId && !hero.IsArchived)
             || await db.PlayerProfiles.AnyAsync(player => player.HeroId == heroId))))
        {
            TempData["PlayerFormError"] = "Revisa el nombre, el apodo y el héroe elegido.";
            return RedirectToAction("Index", "Home");
        }

        db.PlayerProfiles.Add(new PlayerProfile
        {
            Name = input.Name.Trim(),
            Nickname = input.Nickname.Trim(),
            HeroId = input.HeroId
        });
        await db.SaveChangesAsync();
        TempData["PlayerFormMessage"] = "El jugador se ha añadido al registro de la campaña.";
        return RedirectToAction("Index", "Home");
    }
}
