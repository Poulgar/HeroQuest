using Microsoft.EntityFrameworkCore; // Proporciona los métodos y clases necesarios para trabajar con Entity Framework Core.
using HeroQuest.Models; // Importa el espacio de nombres que contiene las clases de modelo.

namespace HeroQuest.Data;

// Clase principal para gestionar la conexión y las entidades con Entity Framework Core
public class ApplicationDbContext : DbContext
{
    public DbSet<Heroe> Heroes => Set<Heroe>(); // Colección de héroes en la base de datos
    public DbSet<Monster> Monsters => Set<Monster>(); // Colección de monstruos
    public DbSet<MiscellaneaItem> Miscellanea => Set<MiscellaneaItem>(); // Objetos variados de la sección Miscelánea
    public DbSet<EquipmentsAndArtifacts> EquipmentsAndArtifacts => Set<EquipmentsAndArtifacts>(); // Colección unificada de equipamiento y artefactos
    public DbSet<MonsterAbility> MonsterAbilities => Set<MonsterAbility>(); // Colección de habilidades de monstruos

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options) // Constructor que recibe opciones de configuración para la base de datos y las pasa a la clase base DbContext.
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) // Método que se llama al crear el modelo de la base de datos, permite configurar las entidades y sus relaciones.
    {
        modelBuilder.Entity<Character>() // Configura la entidad Character en el modelo de la base de datos.
            .UseTpcMappingStrategy() // Configura la estrategia de mapeo TPC (Table Per Concrete Class) para la herencia de la clase Character.
            .Property(c => c.Id) // Configura la propiedad Id de la entidad Character.
            // TPC guarda Heroes y Monsters en tablas separadas. SQLite genera IDs por tabla,
            // lo que puede repetir claves entre ambas tablas; las asignamos globalmente desde los importadores.
            .ValueGeneratedNever();

        modelBuilder.Entity<Heroe>(); // Configura la entidad Heroe en el modelo de la base de datos.
        modelBuilder.Entity<Monster>();
        modelBuilder.Entity<MiscellaneaItem>();
        modelBuilder.Entity<EquipmentsAndArtifacts>();
        modelBuilder.Entity<MonsterAbility>();
    }
}
// Contexto de base de datos que gestiona todas las entidades del proyecto HeroQuest.
