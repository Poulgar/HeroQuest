using Microsoft.EntityFrameworkCore; // Proporciona los métodos y clases necesarios para trabajar con Entity Framework Core.
using HeroQuest.Models; // Importa el espacio de nombres que contiene las clases de modelo.

namespace HeroQuest.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Heroe> Heroes => Set<Heroe>(); // Representa la colección de héroes en la base de datos, permite realizar operaciones CRUD.
    public DbSet<Monster> Monsters => Set<Monster>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options) // Constructor que recibe opciones de configuración para la base de datos y las pasa a la clase base DbContext.
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) // Método que se llama al crear el modelo de la base de datos, permite configurar las entidades y sus relaciones.
    {
        modelBuilder.Entity<Character>() // Configura la entidad Character en el modelo de la base de datos.
            .UseTpcMappingStrategy() // Configura la estrategia de mapeo TPC (Table Per Concrete Class) para la herencia de la clase Character.
            .Property(c => c.Id) // Configura la propiedad Id de la entidad Character.
            .ValueGeneratedOnAdd(); // Indica que el valor de la propiedad Id se generará automáticamente al agregar un nuevo registro a la base de datos.

        modelBuilder.Entity<Heroe>(); // Configura la entidad Heroe en el modelo de la base de datos.
        modelBuilder.Entity<Monster>();
    }
}