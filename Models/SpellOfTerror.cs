namespace HeroQuest.Models;

public class SpellOfTerror(int id, string name, string description, string type, TypeAppears appears) : IArchivable
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public string Type { get; set; } = type;
    public TypeAppears Appears { get; set; } = appears;
    public bool IsArchived { get; set; }
    public ICollection<MonsterSpellOfTerror> MonsterLinks { get; set; } = [];
}
