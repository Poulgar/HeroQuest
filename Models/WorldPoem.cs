namespace HeroQuest.Models;

/// <summary>Versos breves identificados por una clave estable para mostrarlos en distintas partes del sitio.</summary>
public sealed class WorldPoem
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
