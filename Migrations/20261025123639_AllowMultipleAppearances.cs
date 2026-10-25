using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroQuest.Migrations
{
    /// <inheritdoc />
    public partial class AllowMultipleAppearances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Antes el enum se guardaba como una secuencia 0..12; ahora cada expansión ocupa un bit.
            // Actualizamos todas las tablas que contienen Appears para que los registros existentes sigan siendo correctos.
            foreach (var table in AppearanceTables)
            {
                migrationBuilder.Sql($"""
                    UPDATE "{table}" SET "Appears" = CASE "Appears"
                        WHEN 0 THEN 1 WHEN 1 THEN 2 WHEN 2 THEN 4 WHEN 3 THEN 8
                        WHEN 4 THEN 16 WHEN 5 THEN 32 WHEN 6 THEN 64 WHEN 7 THEN 128
                        WHEN 8 THEN 256 WHEN 9 THEN 512 WHEN 10 THEN 1024 WHEN 11 THEN 2048
                        WHEN 12 THEN 4096 ELSE "Appears" END;
                    """);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // La versión anterior sólo admite una expansión; combinaciones se convierten a Original al revertir.
            foreach (var table in AppearanceTables)
            {
                migrationBuilder.Sql($"""
                    UPDATE "{table}" SET "Appears" = CASE "Appears"
                        WHEN 1 THEN 0 WHEN 2 THEN 1 WHEN 4 THEN 2 WHEN 8 THEN 3
                        WHEN 16 THEN 4 WHEN 32 THEN 5 WHEN 64 THEN 6 WHEN 128 THEN 7
                        WHEN 256 THEN 8 WHEN 512 THEN 9 WHEN 1024 THEN 10 WHEN 2048 THEN 11
                        WHEN 4096 THEN 12 ELSE 12 END;
                    """);
            }
        }

        // Esta lista cerrada evita construir nombres de tabla a partir de datos externos.
        private static readonly string[] AppearanceTables =
            ["Heroes", "Monsters", "Miscellanea", "EquipmentsAndArtifacts", "MonsterAbilities"];
    }
}
