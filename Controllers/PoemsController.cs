using HeroQuest.Services;
using Microsoft.AspNetCore.Mvc;

namespace HeroQuest.Controllers;

[Route("Poems")]
public sealed class PoemsController(WorldPoemService poems) : Controller
{
    // La interfaz pide solo el verso por su clave y nunca recibe datos de acceso ni de otras tablas.
    [HttpGet("{key}")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> ByKey(string key)
    {
        var poem = await poems.FindAsync(key);
        return poem is null ? NotFound() : Json(new { poem.Title, poem.Text });
    }
}
