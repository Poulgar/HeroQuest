using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroQuest.Migrations
{
    /// <inheritdoc />
    public partial class RenameItemsToMiscellanea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Renombramos la tabla sin borrarla para conservar todos los registros existentes.
            migrationBuilder.RenameTable(name: "Items", newName: "Miscellanea");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // El rollback también conserva los datos y restaura el nombre anterior.
            migrationBuilder.RenameTable(name: "Miscellanea", newName: "Items");
        }
    }
}
