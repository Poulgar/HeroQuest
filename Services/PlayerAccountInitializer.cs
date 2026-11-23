using System.Text;
using HeroQuest.Data;
using HeroQuest.Models;
using Microsoft.EntityFrameworkCore;

namespace HeroQuest.Services;

public static class PlayerAccountInitializer
{
    public static async Task EnsureAccountsAsync(ApplicationDbContext db)
    {
        var profiles = await db.PlayerProfiles.OrderBy(profile => profile.Id).ToListAsync();
        var usedNames = profiles.Where(profile => !string.IsNullOrWhiteSpace(profile.Username))
            .Select(profile => profile.Username).ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var profile in profiles.Where(profile => string.IsNullOrWhiteSpace(profile.Username)))
        {
            var baseName = MakeUsername(profile.Name);
            var username = baseName;
            var suffix = 2;
            while (usedNames.Contains(username)) username = $"{baseName}{suffix++}";
            profile.Username = username;
            profile.PasswordHash = PasswordHashing.Create(username + "123");
            usedNames.Add(username);
        }

        await db.SaveChangesAsync();
    }

    private static string MakeUsername(string name)
    {
        var decomposed = name.Trim().Normalize(NormalizationForm.FormD);
        var username = new string(decomposed.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
        return string.IsNullOrEmpty(username) ? "aventurero" : username;
    }
}
