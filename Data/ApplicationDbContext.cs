using Microsoft.EntityFrameworkCore; // Proporciona los métodos y clases necesarios para trabajar con Entity Framework Core.
using HeroQuest.Models;

namespace HeroQuest.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Heroe> Heroes => Set<Heroe>(); // Colección de héroes en la base de datos.
    public DbSet<Monster> Monsters => Set<Monster>();
    public DbSet<MiscellaneaItem> Miscellanea => Set<MiscellaneaItem>();
    public DbSet<EquipmentsAndArtifacts> EquipmentsAndArtifacts => Set<EquipmentsAndArtifacts>();
    public DbSet<MonsterAbility> MonsterAbilities => Set<MonsterAbility>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) // Método que se llama al crear el modelo de la base de datos, permite configurar las entidades y sus relaciones.
    {
        modelBuilder.Entity<Character>() // Configura la entidad Character en el modelo de la base de datos.
            .UseTpcMappingStrategy() // Configura la estrategia de mapeo TPC (Table Per Concrete Class) para la herencia de la clase Character.
            .Property(c => c.Id) // Configura la propiedad Id de la entidad Character.
            // TPC guarda Heroes y Monsters en tablas separadas. SQLite genera IDs por tabla,
            // lo que puede repetir claves entre ambas tablas; las asignamos globalmente desde los importadores.
            .ValueGeneratedNever(); // Indica que el valor de la propiedad Id no se generará automáticamente, sino que se asignará manualmente.

        modelBuilder.Entity<Heroe>(); // Configura la entidad Heroe en el modelo de la base de datos.
        modelBuilder.Entity<Monster>();
        modelBuilder.Entity<MiscellaneaItem>();
        modelBuilder.Entity<EquipmentsAndArtifacts>();
        modelBuilder.Entity<MonsterAbility>();
    }
}
