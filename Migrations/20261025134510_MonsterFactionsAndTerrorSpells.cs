using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroQuest.Migrations
{
    /// <inheritdoc />
    public partial class MonsterFactionsAndTerrorSpells : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Hp ya guarda el valor de Cuerpo; Movimiento deja de usarse y TypeMonsterRol queda deshabilitado.
            migrationBuilder.DropColumn(
                name: "Body",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "Movement",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "TypeMonsterRol",
                table: "Monsters");

            // RenameColumn conserva la cantidad de dados rojos ya almacenada para cada héroe.
            migrationBuilder.RenameColumn(
                name: "DicesMovement",
                table: "Heroes",
                newName: "DicesRedMovement");

            migrationBuilder.AddColumn<string>(
                name: "Faction",
                table: "Monsters",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SpellsOfTerror",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Appears = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpellsOfTerror", x => x.Id);
                });

            // La tabla puente implementa la relación muchos-a-muchos con claves externas a ambas tablas.
            migrationBuilder.CreateTable(
                name: "MonsterSpellOfTerror",
                columns: table => new
                {
                    MonsterId = table.Column<int>(type: "INTEGER", nullable: false),
                    SpellOfTerrorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterSpellOfTerror", x => new { x.MonsterId, x.SpellOfTerrorId });
                    table.ForeignKey(
                        name: "FK_MonsterSpellOfTerror_Monsters_MonsterId",
                        column: x => x.MonsterId,
                        principalTable: "Monsters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonsterSpellOfTerror_SpellsOfTerror_SpellOfTerrorId",
                        column: x => x.SpellOfTerrorId,
                        principalTable: "SpellsOfTerror",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonsterSpellOfTerror_SpellOfTerrorId",
                table: "MonsterSpellOfTerror",
                column: "SpellOfTerrorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Las asociaciones y los hechizos nuevos se eliminan al revertir esta migración.
            migrationBuilder.DropTable(
                name: "MonsterSpellOfTerror");

            migrationBuilder.DropTable(
                name: "SpellsOfTerror");

            migrationBuilder.DropColumn(
                name: "Faction",
                table: "Monsters");

            migrationBuilder.RenameColumn(
                name: "DicesRedMovement",
                table: "Heroes",
                newName: "DicesMovement");

            migrationBuilder.AddColumn<int>(
                name: "Body",
                table: "Monsters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Movement",
                table: "Monsters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeMonsterRol",
                table: "Monsters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
