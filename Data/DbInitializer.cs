using Microsoft.EntityFrameworkCore;
using HeroQuest.Models;

namespace HeroQuest.Data;

// Inicializador para poblar la base de datos con datos de prueba
public static class DbInitializer
{
    public static void Initialize(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>(); // Obtiene una instancia del contexto de la base de datos
        context.Database.Migrate(); // Aplica migraciones pendientes

        // Añade monstruos de prueba para poder hacer consultas LINQ
        if (!context.Monsters.Any())
        {
            var nextId = Math.Max(
                context.Heroes.Select(h => h.Id).DefaultIfEmpty(0).Max(),
                context.Monsters.Select(m => m.Id).DefaultIfEmpty(0).Max()) + 1;
            var monsters = new List<Monster>
            {
                // id, characterType, hp, mind, dicesAttack, dicesDefend, maxMovement, name, movement, attack, defense, body, ability, appears
                new(nextId++, "Goblin", 1, 1, 2, 1, 10, "Goblin", 10, 2, 1, 1, "", "Juego Base") { CharacterType = "Goblin", Appears = TypeAppears.JuegoBase },
                new(nextId++, "Orco", 1, 2, 3, 2, 8, "Orco", 8, 3, 2, 1, "", "Juego Base") { CharacterType = "Orco", Appears = TypeAppears.JuegoBase },
                new(nextId++, "Abominación", 2, 3, 3, 3, 6, "Abominación", 6, 3, 3, 2, "", "Juego Base") { CharacterType = "Abominación", Appears = TypeAppears.JuegoBase },
                new(nextId++, "Zombi", 1, 0, 2, 3, 5, "Zombi", 5, 2, 3, 1, "Muerto Viviente", "Juego Base") { CharacterType = "Zombi", Appears = TypeAppears.JuegoBase },
                new(nextId++, "Esqueleto", 1, 0, 2, 2, 6, "Esqueleto", 6, 2, 2, 1, "Muerto Viviente", "Juego Base") { CharacterType = "Esqueleto", Appears = TypeAppears.JuegoBase },
                new(nextId++, "Momia", 2, 0, 3, 4, 4, "Momia", 4, 3, 4, 2, "Muerto Viviente", "Juego Base") { CharacterType = "Momia", Appears = TypeAppears.JuegoBase },
                new(nextId++, "Guerrero del Terror", 3, 3, 4, 4, 7, "Guerrero del Terror", 7, 4, 4, 3, "", "Juego Base") { CharacterType = "Guerrero del Terror", Appears = TypeAppears.JuegoBase },
                new(nextId++, "Gárgola", 3, 4, 4, 5, 6, "Gárgola", 6, 4, 5, 3, "", "Juego Base") { CharacterType = "Gárgola", Appears = TypeAppears.JuegoBase },
                new(nextId++, "Fimir", 2, 3, 3, 3, 6, "Fimir", 6, 3, 3, 2, "", "Juego Base") { CharacterType = "Fimir", Appears = TypeAppears.JuegoBase },
                new(nextId++, "Esqueleto Arquero", 1, 0, 2, 2, 6, "Arquero Esqueleto", 6, 2, 2, 1, "Ataque a Distancia", "Horda del Ogro") { CharacterType = "Arquero Esqueleto", Appears = TypeAppears.HordaDelOgro },
                new(nextId++, "Dragón", 6, 6, 5, 5, 10, "Dragón", 10, 5, 5, 7, "Vuelo Dracónico", "Despertar") { CharacterType = "Dragón", Appears = TypeAppears.Despertar },
                new(nextId++, "Ogro", 5, 2, 6, 4, 4, "Ogro", 4, 6, 4, 5, "", "Maga del Espejo") { CharacterType = "Ogro", Appears = TypeAppears.MagaDelEspejo },
                new(nextId++, "Lobo Gigante", 1, 1, 6, 3, 9, "Lobo Gigante", 9, 6, 3, 5, "", "Maga del Espejo") { CharacterType = "Lobo Gigante", Appears = TypeAppears.MagaDelEspejo },
                new(nextId++, "Horror Congelado", 4, 4, 5, 4, 8, "Horror Congelado", 8, 5, 4, 6, "Ataque Doble", "Horror Congelado") { CharacterType = "Horror Congelado", Appears = TypeAppears.HorrorCongelado },
                new(nextId++, "Gremlin de Hielo", 3, 3, 2, 3, 10, "Gremlin de Hielo", 10, 2, 3, 3, "Vuelo Dracónico", "Horror Congelado") { CharacterType = "Gremlin de Hielo", Appears = TypeAppears.HorrorCongelado }
            };

            context.Monsters.AddRange(monsters);
            context.SaveChanges();
        }
    }
}

