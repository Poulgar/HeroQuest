using Microsoft.EntityFrameworkCore;
using HeroQuest.Models;

namespace HeroQuest.Data;

public static class DbInitializer
{
    public static void Initialize(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>(); // Obtiene una instancia del contexto de la base de datos desde el proveedor de servicios.
        context.Database.Migrate(); // Aplica cualquier migración pendiente a la base de datos, asegurando que esté actualizada con el modelo actual.

        if(!context.Monsters.Any()) 
        {
            var monsters = new List<Monster>
            {
                // id, characterType, hp, mind, dicesAttack, dicesDefend, maxMovement
                new(0, "Goblin", 1, 1, 2, 1, 7) { CharacterType = "Goblin" },
                new(0, "Orco", 1, 2, 3, 2, 8) { CharacterType = "Orco" },
                new(0, "Abominación", 2, 3, 3, 3, 6) { CharacterType = "Abominación" },
                new(0, "Zombi", 1, 0, 2, 3, 5) { CharacterType = "Zombi" },
                new(0, "Esqueleto", 1, 0, 2, 2, 6) { CharacterType = "Esqueleto" },
                new(0, "Momia", 2, 0, 3, 4, 4) { CharacterType = "Momia" },
                new(0, "Guerrero del Terror", 3, 3, 4, 4, 7) { CharacterType = "Guerrero del Terror" },
                new(0, "Gárgola", 3, 4, 4, 5, 6) { CharacterType = "Gárgola" },
                new(0, "Fimir", 2, 3, 3, 3, 6) { CharacterType = "Fimir" }                
            };

            context.Monsters.AddRange(monsters);
            context.SaveChanges();
        }
    }
}