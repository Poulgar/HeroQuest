using System.ComponentModel.DataAnnotations;

namespace HeroQuest.Models;

/// <summary>Identifica a una persona jugadora y permite asociarla opcionalmente con un héroe de la biblioteca.</summary>
public class PlayerProfile
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(60)]
    public string Nickname { get; set; } = string.Empty;

    // TPC guarda héroes y monstruos en tablas distintas; por ahora un jugador controla un héroe concreto.
    public int? HeroId { get; set; }
    public Heroe? Hero { get; set; }
}
