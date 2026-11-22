namespace HeroQuest.Models;

/// <summary>Contrato común para las fichas que pueden ocultarse temporalmente del compendio.</summary>
public interface IArchivable
{
    bool IsArchived { get; set; }
}
