using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroQuest.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Character",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    CharacterType = table.Column<string>(type: "TEXT", nullable: false),
                    Hp = table.Column<int>(type: "INTEGER", nullable: false),
                    HpCurrent = table.Column<int>(type: "INTEGER", nullable: false),
                    Mind = table.Column<int>(type: "INTEGER", nullable: false),
                    DicesAttack = table.Column<int>(type: "INTEGER", nullable: false),
                    DicesDefend = table.Column<int>(type: "INTEGER", nullable: false),
                    DicesMovement = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Heroes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    CharacterType = table.Column<string>(type: "TEXT", nullable: false),
                    Hp = table.Column<int>(type: "INTEGER", nullable: false),
                    HpCurrent = table.Column<int>(type: "INTEGER", nullable: false),
                    Mind = table.Column<int>(type: "INTEGER", nullable: false),
                    DicesAttack = table.Column<int>(type: "INTEGER", nullable: false),
                    DicesDefend = table.Column<int>(type: "INTEGER", nullable: false),
                    DicesMovement = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Player = table.Column<string>(type: "TEXT", nullable: false),
                    Weapon = table.Column<string>(type: "TEXT", nullable: false),
                    Armor = table.Column<string>(type: "TEXT", nullable: true),
                    Spells = table.Column<string>(type: "TEXT", nullable: true),
                    Gold = table.Column<int>(type: "INTEGER", nullable: false),
                    Items = table.Column<string>(type: "TEXT", nullable: true),
                    QuestsCompleted = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heroes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monsters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    CharacterType = table.Column<string>(type: "TEXT", nullable: false),
                    Hp = table.Column<int>(type: "INTEGER", nullable: false),
                    HpCurrent = table.Column<int>(type: "INTEGER", nullable: false),
                    Mind = table.Column<int>(type: "INTEGER", nullable: false),
                    DicesAttack = table.Column<int>(type: "INTEGER", nullable: false),
                    DicesDefend = table.Column<int>(type: "INTEGER", nullable: false),
                    DicesMovement = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsters", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Character");

            migrationBuilder.DropTable(
                name: "Heroes");

            migrationBuilder.DropTable(
                name: "Monsters");
        }
    }
}
