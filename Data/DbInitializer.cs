using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Data;

public static class DbInitializer
{
    public static void Initialize(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>(); // Obtiene una instancia del contexto de la base de datos desde el proveedor de servicios.
        context.Database.Migrate(); // Aplica cualquier migración pendiente a la base de datos, asegurando que esté actualizada con el modelo actual.
    }
}