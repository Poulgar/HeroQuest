using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroQuest.Migrations
{
    /// <inheritdoc />
    public partial class AddWorldPoems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorldPoems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldPoems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorldPoems_Key",
                table: "WorldPoems",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorldPoems");
        }
    }
}
