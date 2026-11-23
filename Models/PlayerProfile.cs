using System.ComponentModel.DataAnnotations;

namespace HeroQuest.Models;

/// <summary>Cuenta normal de una persona jugadora; sus fichas personales se guardan por héroe y partida.</summary>
public class PlayerProfile
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [StringLength(60)]
    public string? Nickname { get; set; }

    [Required, StringLength(60)]
    public string Username { get; set; } = string.Empty;

    // Sólo se almacena el resultado PBKDF2; la contraseña de acceso nunca se guarda en claro.
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<HeroConfiguration> HeroConfigurations { get; set; } = [];
}
