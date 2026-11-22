using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroQuest.Migrations
{
    /// <inheritdoc />
    public partial class AddMiscellaneaCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Miscellanea",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Abrojos', Category = 2 WHERE Name = 'Abrojos (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Agua Bendita';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Antídoto contra Venenos';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Astilla de Madera';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Baraja de Póker';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Bomba de Humo', Category = 2 WHERE Name = 'Bomba de Humo (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Capa Élfica de Paso', Category = 4 WHERE Name = 'Capa Élfica de Paso (Tesoro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Celeridad', Category = 24 WHERE Name = 'Celeridad (Alquimia — Elaborada)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Conjunto de Herramientas';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Cráneo de Esqueleto';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Cuerno de Gárgola';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Elixir de Pasos de Araña', Category = 8 WHERE Name = 'Elixir de Pasos de Araña (Alquimia)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Estuche de Reactivos', Category = 2 WHERE Name = 'Estuche de Reactivos (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Flor Misteriosa', Category = 4 WHERE Name = 'Flor Misteriosa (Tesoro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Grog Ogro', Category = 10 WHERE Name = 'Grog Ogro (Alquimia) (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Ingrediente Desconocido', Category = 4 WHERE Name = 'Ingrediente Desconocido (Tesoro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Planta Sagrada', Category = 4 WHERE Name = 'Planta Sagrada (Tesoro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Poción Curativa';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción Curativa', Category = 10 WHERE Name = 'Poción Curativa (Alquimia) (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción Curativa', Category = 4 WHERE Name = 'Poción Curativa (Tesoro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Poción Curativa Menor';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción Curativa Menor', Category = 10 WHERE Name = 'Poción Curativa Menor (Alquimia) (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Poción Curativa Superior';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Poción de Batalla/Combate';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Calidez', Category = 4 WHERE Name = 'Poción de Calidez (Tesoro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Combate', Category = 10 WHERE Name = 'Poción de Combate (Alquimia) (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Curación de la Maldición del Hombre Lobo', Category = 4 WHERE Name = 'Poción de Curación de la Maldición del Hombre Lobo (Tesoro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Defensa', Category = 24 WHERE Name = 'Poción de Defensa (Alquimia — Elaborada)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Defensa', Category = 4 WHERE Name = 'Poción de Defensa (Tesoro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Destino Imprevisible', Category = 24 WHERE Name = 'Poción de Destino Imprevisible (Alquimia - Elaborada)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Poción de Destreza';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Destreza', Category = 10 WHERE Name = 'Poción de Destreza (Alquimia) (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Fuerza', Category = 8 WHERE Name = 'Poción de Fuerza (Alquimia)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Fuerza', Category = 4 WHERE Name = 'Poción de Fuerza (Tesoro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Fuerza Gélida', Category = 10 WHERE Name = 'Poción de Fuerza Gélida (Alquimia) (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Fuerza Gélida', Category = 64 WHERE Name = 'Poción de Fuerza Gélida (Bárbaro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Furia de Batalla', Category = 64 WHERE Name = 'Poción de Furia de Batalla (Bárbaro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Furia de Combate', Category = 10 WHERE Name = 'Poción de Furia de Combate (Alquimia) (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de la Sabiduría Ancestral', Category = 8 WHERE Name = 'Poción de la Sabiduría Ancestral (Alquimia)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Poción de Magia';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Magia', Category = 24 WHERE Name = 'Poción de Magia (Alquimia - Elaborada)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Paseo Aéreo', Category = 4 WHERE Name = 'Poción de Paseo Aéreo (Tesoro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Piel Congelada', Category = 64 WHERE Name = 'Poción de Piel Congelada (Bárbaro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Piel Congelada', Category = 2 WHERE Name = 'Poción de Piel Congelada (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Poción de Rejuvenecimiento';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Resistencia Mágica', Category = 24 WHERE Name = 'Poción de Resistencia Mágica (Alquimia - Elaborada)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Resistencia Mágica', Category = 4 WHERE Name = 'Poción de Resistencia Mágica (Tesoro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Poción de Restablecimiento';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Restablecimiento', Category = 10 WHERE Name = 'Poción de Restablecimiento (Alquimia) (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Poción de Restauración';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Poción de Restauración Superior';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Restauración Superior', Category = 10 WHERE Name = 'Poción de Restauración Superior (Alquimia) (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Poción de Sanación';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Sangre de Serpiente', Category = 8 WHERE Name = 'Poción de Sangre de Serpiente (Alquimia)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Poción de Velocidad';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Velocidad', Category = 10 WHERE Name = 'Poción de Velocidad (Alquimia) (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Velocidad', Category = 32 WHERE Name = 'Poción de Velocidad (Elfo)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Visión', Category = 10 WHERE Name = 'Poción de Visión (Alquimia) (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Visión', Category = 32 WHERE Name = 'Poción de Visión (Elfo)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción del Recuerdo', Category = 10 WHERE Name = 'Poción del Recuerdo (Alquimia) (Mercado Negro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción del Recuerdo', Category = 32 WHERE Name = 'Poción del Recuerdo (Elfo)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción Heroica', Category = 24 WHERE Name = 'Poción Heroica (Alquimia — Elaborada)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción Heroica', Category = 4 WHERE Name = 'Poción Heroica (Tesoro)';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Poción Petrificante';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Poción Poder Inverso';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Trozo Afilado de Metal Oxidado';");
            migrationBuilder.Sql("UPDATE Miscellanea SET Category = 1 WHERE Name = 'Vendaje de Momia';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción Heroica (Tesoro)', Category = 1 WHERE Name = 'Poción Heroica' AND Category = 4;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción Heroica (Alquimia — Elaborada)', Category = 1 WHERE Name = 'Poción Heroica' AND Category = 24;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción del Recuerdo (Elfo)', Category = 1 WHERE Name = 'Poción del Recuerdo' AND Category = 32;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción del Recuerdo (Alquimia) (Mercado Negro)', Category = 1 WHERE Name = 'Poción del Recuerdo' AND Category = 10;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Visión (Elfo)', Category = 1 WHERE Name = 'Poción de Visión' AND Category = 32;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Visión (Alquimia) (Mercado Negro)', Category = 1 WHERE Name = 'Poción de Visión' AND Category = 10;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Velocidad (Elfo)', Category = 1 WHERE Name = 'Poción de Velocidad' AND Category = 32;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Velocidad (Alquimia) (Mercado Negro)', Category = 1 WHERE Name = 'Poción de Velocidad' AND Category = 10;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Sangre de Serpiente (Alquimia)', Category = 1 WHERE Name = 'Poción de Sangre de Serpiente' AND Category = 8;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Restauración Superior (Alquimia) (Mercado Negro)', Category = 1 WHERE Name = 'Poción de Restauración Superior' AND Category = 10;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Restablecimiento (Alquimia) (Mercado Negro)', Category = 1 WHERE Name = 'Poción de Restablecimiento' AND Category = 10;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Resistencia Mágica (Tesoro)', Category = 1 WHERE Name = 'Poción de Resistencia Mágica' AND Category = 4;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Resistencia Mágica (Alquimia - Elaborada)', Category = 1 WHERE Name = 'Poción de Resistencia Mágica' AND Category = 24;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Piel Congelada (Mercado Negro)', Category = 1 WHERE Name = 'Poción de Piel Congelada' AND Category = 2;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Piel Congelada (Bárbaro)', Category = 1 WHERE Name = 'Poción de Piel Congelada' AND Category = 64;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Paseo Aéreo (Tesoro)', Category = 1 WHERE Name = 'Poción de Paseo Aéreo' AND Category = 4;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Magia (Alquimia - Elaborada)', Category = 1 WHERE Name = 'Poción de Magia' AND Category = 24;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de la Sabiduría Ancestral (Alquimia)', Category = 1 WHERE Name = 'Poción de la Sabiduría Ancestral' AND Category = 8;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Furia de Combate (Alquimia) (Mercado Negro)', Category = 1 WHERE Name = 'Poción de Furia de Combate' AND Category = 10;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Furia de Batalla (Bárbaro)', Category = 1 WHERE Name = 'Poción de Furia de Batalla' AND Category = 64;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Fuerza Gélida (Bárbaro)', Category = 1 WHERE Name = 'Poción de Fuerza Gélida' AND Category = 64;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Fuerza Gélida (Alquimia) (Mercado Negro)', Category = 1 WHERE Name = 'Poción de Fuerza Gélida' AND Category = 10;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Fuerza (Tesoro)', Category = 1 WHERE Name = 'Poción de Fuerza' AND Category = 4;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Fuerza (Alquimia)', Category = 1 WHERE Name = 'Poción de Fuerza' AND Category = 8;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Destreza (Alquimia) (Mercado Negro)', Category = 1 WHERE Name = 'Poción de Destreza' AND Category = 10;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Destino Imprevisible (Alquimia - Elaborada)', Category = 1 WHERE Name = 'Poción de Destino Imprevisible' AND Category = 24;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Defensa (Tesoro)', Category = 1 WHERE Name = 'Poción de Defensa' AND Category = 4;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Defensa (Alquimia — Elaborada)', Category = 1 WHERE Name = 'Poción de Defensa' AND Category = 24;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Curación de la Maldición del Hombre Lobo (Tesoro)', Category = 1 WHERE Name = 'Poción de Curación de la Maldición del Hombre Lobo' AND Category = 4;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Combate (Alquimia) (Mercado Negro)', Category = 1 WHERE Name = 'Poción de Combate' AND Category = 10;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción de Calidez (Tesoro)', Category = 1 WHERE Name = 'Poción de Calidez' AND Category = 4;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción Curativa Menor (Alquimia) (Mercado Negro)', Category = 1 WHERE Name = 'Poción Curativa Menor' AND Category = 10;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción Curativa (Tesoro)', Category = 1 WHERE Name = 'Poción Curativa' AND Category = 4;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Poción Curativa (Alquimia) (Mercado Negro)', Category = 1 WHERE Name = 'Poción Curativa' AND Category = 10;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Planta Sagrada (Tesoro)', Category = 1 WHERE Name = 'Planta Sagrada' AND Category = 4;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Ingrediente Desconocido (Tesoro)', Category = 1 WHERE Name = 'Ingrediente Desconocido' AND Category = 4;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Grog Ogro (Alquimia) (Mercado Negro)', Category = 1 WHERE Name = 'Grog Ogro' AND Category = 10;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Flor Misteriosa (Tesoro)', Category = 1 WHERE Name = 'Flor Misteriosa' AND Category = 4;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Estuche de Reactivos (Mercado Negro)', Category = 1 WHERE Name = 'Estuche de Reactivos' AND Category = 2;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Elixir de Pasos de Araña (Alquimia)', Category = 1 WHERE Name = 'Elixir de Pasos de Araña' AND Category = 8;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Celeridad (Alquimia — Elaborada)', Category = 1 WHERE Name = 'Celeridad' AND Category = 24;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Capa Élfica de Paso (Tesoro)', Category = 1 WHERE Name = 'Capa Élfica de Paso' AND Category = 4;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Bomba de Humo (Mercado Negro)', Category = 1 WHERE Name = 'Bomba de Humo' AND Category = 2;");
            migrationBuilder.Sql("UPDATE Miscellanea SET Name = 'Abrojos (Mercado Negro)', Category = 1 WHERE Name = 'Abrojos' AND Category = 2;");
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Miscellanea");
        }
    }
}
