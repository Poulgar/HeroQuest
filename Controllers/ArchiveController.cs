using Microsoft.AspNetCore.Mvc;

namespace HeroQuest.Controllers;

public class ArchiveController : Controller
{
    public IActionResult Equipment()
    {
        ViewData["Title"] = "Equipo y artefactos";
        return View("ComingSoon", "Equipo y artefactos");
    }

    public IActionResult Modifiers()
    {
        ViewData["Title"] = "Modificadores";
        return View("ComingSoon", "Modificadores");
    }
}
