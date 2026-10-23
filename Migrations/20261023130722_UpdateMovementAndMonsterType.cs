using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroQuest.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMovementAndMonsterType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DicesMovement",
                table: "Character");

            migrationBuilder.RenameColumn(
                name: "DicesMovement",
                table: "Monsters",
                newName: "TypeMonsterRol");

            migrationBuilder.AddColumn<int>(
                name: "MaxMovement",
                table: "Monsters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxMovement",
                table: "Monsters");

            migrationBuilder.RenameColumn(
                name: "TypeMonsterRol",
                table: "Monsters",
                newName: "DicesMovement");

            migrationBuilder.AddColumn<int>(
                name: "DicesMovement",
                table: "Character",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
