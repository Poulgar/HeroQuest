using System.Security.Claims;
using HeroQuest.Data;
using HeroQuest.Models;
using HeroQuest.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Controllers;

public class AccountController(IConfiguration configuration, ApplicationDbContext db) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        // Estas cuentas son únicamente para una demo local; no reutilices estas claves en un sitio público.
        var account = configuration.GetSection("DemoAccounts").GetChildren()
            .FirstOrDefault(candidate => string.Equals(candidate["Username"], model.Username.Trim(), StringComparison.OrdinalIgnoreCase));
        if (account is not null && PasswordHashing.Verify(model.Password, account["PasswordHash"]))
        {
            var username = account["Username"]!;
            var role = account["Role"]!;
            return await SignInAsync(username, role, null, returnUrl);
        }

        // Los usuarios normales creados por el Maestro de Juego tienen su cuenta guardada en SQLite.
        var player = await db.PlayerProfiles.AsNoTracking()
            .FirstOrDefaultAsync(profile => profile.Username.ToLower() == model.Username.Trim().ToLower());
        if (player is null || !PasswordHashing.Verify(model.Password, player.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "El usuario o la contraseña no son correctos.");
            return View(model);
        }

        return await SignInAsync(player.Username, "User", player.Id, returnUrl);
    }

    private async Task<IActionResult> SignInAsync(string username, string role, int? profileId, string? returnUrl)
    {
        var claims = new List<Claim> { new(ClaimTypes.Name, username), new(ClaimTypes.Role, role) };
        if (profileId.HasValue) claims.Add(new Claim(ClaimTypes.NameIdentifier, profileId.Value.ToString()));
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl!) : RedirectToAction("Index", "Home");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Denied() => View();
}
