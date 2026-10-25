namespace HeroQuest.Models;

public class MiscellaneaItem(int id, string name, string description, int price, TypeAppears appears)
{
    // Entidad de la sección Miscelánea: conserva el mismo esquema de datos de la antigua tabla Items.
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public int Price { get; set; } = price;
    public TypeAppears Appears { get; set; } = appears;
}
