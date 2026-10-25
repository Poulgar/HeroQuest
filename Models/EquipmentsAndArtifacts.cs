namespace HeroQuest.Models;

public class EquipmentsAndArtifacts(int id, string name, string description, int price, string type, TypeAppears appears, string weight, string category)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public int Price { get; set; } = price;
    public string Type { get; set; } = type;
    public TypeAppears Appears { get; set; } = appears;
    public string Weight { get; set; } = weight;
    public string Category { get; set; } = category; // "Equipment" or "Artifact"
}
