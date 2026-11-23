using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroQuest.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerHeroConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Conserva las asociaciones antiguas antes de retirar HeroId de PlayerProfiles.
            migrationBuilder.CreateTable(
                name: "LegacyHeroProfileLinks",
                columns: table => new
                {
                    PlayerProfileId = table.Column<int>(type: "INTEGER", nullable: false),
                    HeroId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_LegacyHeroProfileLinks", x => new { x.PlayerProfileId, x.HeroId }));
            migrationBuilder.Sql("INSERT INTO LegacyHeroProfileLinks (PlayerProfileId, HeroId) SELECT Id, HeroId FROM PlayerProfiles WHERE HeroId IS NOT NULL;");
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerProfiles_Heroes_HeroId",
                table: "PlayerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_PlayerProfiles_HeroId",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "HeroId",
                table: "PlayerProfiles");

            migrationBuilder.AlterColumn<string>(
                name: "Nickname",
                table: "PlayerProfiles",
                type: "TEXT",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 60);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "PlayerProfiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "PlayerProfiles",
                type: "TEXT",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AllowedClasses",
                table: "Miscellanea",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CannotCombineWith",
                table: "Miscellanea",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DefenseBonus",
                table: "Miscellanea",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentSlot",
                table: "Miscellanea",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExcludedClasses",
                table: "Miscellanea",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "HealthBonus",
                table: "Miscellanea",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsRangedWeapon",
                table: "Miscellanea",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MindBonus",
                table: "Miscellanea",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MovementDiceOverride",
                table: "Miscellanea",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MovementSquaresBonus",
                table: "Miscellanea",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeaponAttackDice",
                table: "Miscellanea",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AllowedClasses",
                table: "EquipmentsAndArtifacts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CannotCombineWith",
                table: "EquipmentsAndArtifacts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DefenseBonus",
                table: "EquipmentsAndArtifacts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentSlot",
                table: "EquipmentsAndArtifacts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExcludedClasses",
                table: "EquipmentsAndArtifacts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "HealthBonus",
                table: "EquipmentsAndArtifacts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsRangedWeapon",
                table: "EquipmentsAndArtifacts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MindBonus",
                table: "EquipmentsAndArtifacts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MovementDiceOverride",
                table: "EquipmentsAndArtifacts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MovementSquaresBonus",
                table: "EquipmentsAndArtifacts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeaponAttackDice",
                table: "EquipmentsAndArtifacts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HeroConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HeroId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerProfileId = table.Column<int>(type: "INTEGER", nullable: false),
                    CharacterName = table.Column<string>(type: "TEXT", nullable: false),
                    Weapons = table.Column<string>(type: "TEXT", nullable: false),
                    Armors = table.Column<string>(type: "TEXT", nullable: false),
                    Items = table.Column<string>(type: "TEXT", nullable: false),
                    Abilities = table.Column<string>(type: "TEXT", nullable: false),
                    MovementDice = table.Column<int>(type: "INTEGER", nullable: false),
                    Gold = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestsCompleted = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroConfigurations_Heroes_HeroId",
                        column: x => x.HeroId,
                        principalTable: "Heroes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeroConfigurations_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Traslada cada relación previa con su equipo y estado; no se pierde el héroe que tenía asignado cada jugador.
            migrationBuilder.Sql("""
                INSERT INTO HeroConfigurations
                    (HeroId, PlayerProfileId, CharacterName, Weapons, Armors, Items, Abilities, MovementDice, Gold, QuestsCompleted, IsActive)
                SELECT h.Id, p.Id, CASE WHEN trim(p.Nickname) <> '' THEN p.Nickname ELSE p.Name END,
                    h.Weapon, coalesce(h.Armor, ''), coalesce(h.Items, ''), coalesce(h.Spells, ''),
                    h.DicesRedMovement, h.Gold, h.QuestsCompleted, 1
                FROM LegacyHeroProfileLinks link
                JOIN PlayerProfiles p ON p.Id = link.PlayerProfileId
                JOIN Heroes h ON h.Id = link.HeroId;
                """);
            migrationBuilder.DropTable(name: "LegacyHeroProfileLinks");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerProfiles_Username",
                table: "PlayerProfiles",
                column: "Username",
                unique: true,
                filter: "\"Username\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_HeroConfigurations_HeroId",
                table: "HeroConfigurations",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroConfigurations_PlayerProfileId_HeroId",
                table: "HeroConfigurations",
                columns: new[] { "PlayerProfileId", "HeroId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeroConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_PlayerProfiles_Username",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "AllowedClasses",
                table: "Miscellanea");

            migrationBuilder.DropColumn(
                name: "CannotCombineWith",
                table: "Miscellanea");

            migrationBuilder.DropColumn(
                name: "DefenseBonus",
                table: "Miscellanea");

            migrationBuilder.DropColumn(
                name: "EquipmentSlot",
                table: "Miscellanea");

            migrationBuilder.DropColumn(
                name: "ExcludedClasses",
                table: "Miscellanea");

            migrationBuilder.DropColumn(
                name: "HealthBonus",
                table: "Miscellanea");

            migrationBuilder.DropColumn(
                name: "IsRangedWeapon",
                table: "Miscellanea");

            migrationBuilder.DropColumn(
                name: "MindBonus",
                table: "Miscellanea");

            migrationBuilder.DropColumn(
                name: "MovementDiceOverride",
                table: "Miscellanea");

            migrationBuilder.DropColumn(
                name: "MovementSquaresBonus",
                table: "Miscellanea");

            migrationBuilder.DropColumn(
                name: "WeaponAttackDice",
                table: "Miscellanea");

            migrationBuilder.DropColumn(
                name: "AllowedClasses",
                table: "EquipmentsAndArtifacts");

            migrationBuilder.DropColumn(
                name: "CannotCombineWith",
                table: "EquipmentsAndArtifacts");

            migrationBuilder.DropColumn(
                name: "DefenseBonus",
                table: "EquipmentsAndArtifacts");

            migrationBuilder.DropColumn(
                name: "EquipmentSlot",
                table: "EquipmentsAndArtifacts");

            migrationBuilder.DropColumn(
                name: "ExcludedClasses",
                table: "EquipmentsAndArtifacts");

            migrationBuilder.DropColumn(
                name: "HealthBonus",
                table: "EquipmentsAndArtifacts");

            migrationBuilder.DropColumn(
                name: "IsRangedWeapon",
                table: "EquipmentsAndArtifacts");

            migrationBuilder.DropColumn(
                name: "MindBonus",
                table: "EquipmentsAndArtifacts");

            migrationBuilder.DropColumn(
                name: "MovementDiceOverride",
                table: "EquipmentsAndArtifacts");

            migrationBuilder.DropColumn(
                name: "MovementSquaresBonus",
                table: "EquipmentsAndArtifacts");

            migrationBuilder.DropColumn(
                name: "WeaponAttackDice",
                table: "EquipmentsAndArtifacts");

            migrationBuilder.AlterColumn<string>(
                name: "Nickname",
                table: "PlayerProfiles",
                type: "TEXT",
                maxLength: 60,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 60,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeroId",
                table: "PlayerProfiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerProfiles_HeroId",
                table: "PlayerProfiles",
                column: "HeroId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerProfiles_Heroes_HeroId",
                table: "PlayerProfiles",
                column: "HeroId",
                principalTable: "Heroes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
