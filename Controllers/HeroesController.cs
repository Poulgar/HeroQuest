using HeroQuest.Data;
using HeroQuest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Controllers;

public class HeroesController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index(int? id)
    {
        var heroes = await context.Heroes.AsNoTracking().OrderBy(hero => hero.Id).ToListAsync(); // Se obtiene la lista de héroes de la base de datos, ordenada por su Id, y se almacena en la variable heroes. <<AsNoTracking()>> se utiliza para mejorar el rendimiento al no realizar un seguimiento de los cambios en las entidades recuperadas. <<ToListAsync()>> convierte el resultado en una lista de manera asincrónica.

        var selected = heroes.FirstOrDefault(hero => hero.Id == id) ?? heroes.FirstOrDefault(); // Se busca el héroe con el Id proporcionado (si existe) y se almacena en la variable selected. Si no se encuentra ningún héroe con ese Id, se selecciona el primer héroe de la lista. <<FirstOrDefault()>> devuelve el primer elemento de la secuencia o un valor predeterminado si no hay elementos.
        ViewData["Title"] = "Héroes"; // Se establece el título de la página en "Héroes" utilizando ViewData, que es un diccionario que permite pasar datos desde el controlador a la vista.

        return View("HeroesIndex", new HeroCatalogViewModel { Heroes = heroes, SelectedHero = selected }); // Se devuelve la vista "HeroesIndex" junto con un modelo de vista (HeroCatalogViewModel) que contiene la lista de héroes y el héroe seleccionado. Esto permite que la vista tenga acceso a los datos necesarios para mostrar la información de los héroes y resaltar el héroe seleccionado.
    }
}
