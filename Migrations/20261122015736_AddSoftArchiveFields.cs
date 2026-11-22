using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroQuest.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftArchiveFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "SpellsOfTerror",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Monsters",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "MonsterAbilities",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Miscellanea",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Heroes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "EquipmentsAndArtifacts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Character",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "SpellsOfTerror");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "MonsterAbilities");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Miscellanea");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Heroes");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "EquipmentsAndArtifacts");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Character");
        }
    }
}
