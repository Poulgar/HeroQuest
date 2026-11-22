using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroQuest.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterSourceNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceName",
                table: "Monsters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceName",
                table: "Heroes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceName",
                table: "Character",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceName",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "SourceName",
                table: "Heroes");

            migrationBuilder.DropColumn(
                name: "SourceName",
                table: "Character");
        }
    }
}
