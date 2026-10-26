using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroQuest.Migrations
{
    /// <inheritdoc />
    public partial class RenameCharacterStatsAndDropHpCurrent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // La vida actual no forma parte del modelo informativo; renombramos las columnas restantes sin recrear las tablas.
            migrationBuilder.DropColumn(
                name: "HpCurrent",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "HpCurrent",
                table: "Heroes");

            migrationBuilder.DropColumn(
                name: "HpCurrent",
                table: "Character");

            migrationBuilder.RenameColumn(
                name: "Mind",
                table: "Monsters",
                newName: "DicesMind");

            migrationBuilder.RenameColumn(
                name: "CharacterType",
                table: "Monsters",
                newName: "CharacterTypeClass");

            migrationBuilder.RenameColumn(
                name: "Mind",
                table: "Heroes",
                newName: "DicesMind");

            migrationBuilder.RenameColumn(
                name: "CharacterType",
                table: "Heroes",
                newName: "CharacterTypeClass");

            migrationBuilder.RenameColumn(
                name: "Mind",
                table: "Character",
                newName: "DicesMind");

            migrationBuilder.RenameColumn(
                name: "CharacterType",
                table: "Character",
                newName: "CharacterTypeClass");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Al revertir se recupera HpCurrent desde la vida máxima, ya que la propiedad eliminada no guardaba progreso de campaña.
            migrationBuilder.RenameColumn(
                name: "DicesMind",
                table: "Monsters",
                newName: "Mind");

            migrationBuilder.RenameColumn(
                name: "CharacterTypeClass",
                table: "Monsters",
                newName: "CharacterType");

            migrationBuilder.RenameColumn(
                name: "DicesMind",
                table: "Heroes",
                newName: "Mind");

            migrationBuilder.RenameColumn(
                name: "CharacterTypeClass",
                table: "Heroes",
                newName: "CharacterType");

            migrationBuilder.RenameColumn(
                name: "DicesMind",
                table: "Character",
                newName: "Mind");

            migrationBuilder.RenameColumn(
                name: "CharacterTypeClass",
                table: "Character",
                newName: "CharacterType");

            migrationBuilder.AddColumn<int>(
                name: "HpCurrent",
                table: "Monsters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HpCurrent",
                table: "Heroes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HpCurrent",
                table: "Character",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Heroes SET HpCurrent = Hp;");
            migrationBuilder.Sql("UPDATE Monsters SET HpCurrent = Hp;");
            migrationBuilder.Sql("UPDATE Character SET HpCurrent = Hp;");
        }
    }
}
