namespace HeroQuest.Models;

public class MonsterAbility(int id, string name, string description, TypeAppears appears)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public TypeAppears Appears { get; set; } = appears;
}
