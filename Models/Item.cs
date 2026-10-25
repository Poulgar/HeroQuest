namespace HeroQuest.Models;

public class Item(int id, string name, string description, int price, TypeAppears appears)
{
    // Modificadores, mas tarde hay que revisarlo y consultarlo con Arnau.
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public int Price { get; set; } = price;
    public TypeAppears Appears { get; set; } = appears;
}
