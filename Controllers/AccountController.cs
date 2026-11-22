using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HeroQuest.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace HeroQuest.Controllers;

public class AccountController(IConfiguration configuration) : Controller
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
        if (account is null || !VerifyPassword(model.Password, account["PasswordHash"]))
        {
            ModelState.AddModelError(string.Empty, "El usuario o la contraseña no son correctos.");
            return View(model);
        }

        var username = account["Username"]!;
        var role = account["Role"]!;
        var claims = new[] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, role) };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl!) : RedirectToAction("Index", "Home");
    }

    // PBKDF2 mantiene solo el hash en appsettings; FixedTimeEquals evita filtrar si partes coinciden antes.
    private static bool VerifyPassword(string password, string? storedHash)
    {
        var parts = storedHash?.Split('$');
        if (parts is not { Length: 3 } || !int.TryParse(parts[0], out var iterations)) return false;
        try
        {
            var salt = Convert.FromBase64String(parts[1]);
            var expected = Convert.FromBase64String(parts[2]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException) { return false; }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Denied() => View();
}
