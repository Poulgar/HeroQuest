using HeroQuest.Data;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Services;

/// <summary>Relee los textos existentes para que una migración nueva también sirva con las fichas ya guardadas.</summary>
public sealed class EquipmentEffectService(ApplicationDbContext db)
{
    public async Task RefreshAsync()
    {
        foreach (var item in await db.EquipmentsAndArtifacts.ToListAsync()) EquipmentEffectParser.Apply(item);
        foreach (var item in await db.Miscellanea.ToListAsync()) EquipmentEffectParser.Apply(item);
        await db.SaveChangesAsync();
    }
}
