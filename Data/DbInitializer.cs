using HeroQuest.Models;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Data;

// Seed local de monstruos de prueba; la carga principal del proyecto se hace desde los CSV.
public static class DbInitializer
{
    public static void Initialize(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        if (context.Monsters.Any()) return;

        // Los IDs de héroes y monstruos pertenecen a la misma jerarquía TPC y deben ser globalmente únicos.
        var nextId = Math.Max(
            context.Heroes.Select(hero => hero.Id).DefaultIfEmpty(0).Max(),
            context.Monsters.Select(monster => monster.Id).DefaultIfEmpty(0).Max()) + 1;

        var monsters = new List<Monster>
        {
            new(nextId++, "Goblin", 1, 1, 2, 1, 10, "", "Pieles Verdes", "Un goblin ágil que acecha en los pasadizos de la mazmorra."),
            new(nextId++, "Orco", 1, 2, 3, 2, 8, "", "Pieles Verdes", "Un guerrero brutal de la facción de los Pieles Verdes."),
            new(nextId++, "Abominación", 2, 3, 3, 3, 6, "", "Pieles Verdes", "Una criatura poderosa que protege los dominios de su facción."),
            new(nextId++, "Zombi", 1, 0, 2, 3, 5, "Muerto Viviente", "Muertos Vivientes", "Un cadáver reanimado que avanza sin temor por la mazmorra."),
            new(nextId++, "Esqueleto", 1, 0, 2, 2, 6, "Muerto Viviente", "Muertos Vivientes", "Huesos animados por una fuerza oscura y persistente.")
        };

        context.Monsters.AddRange(monsters);
        context.SaveChanges();
    }
}
