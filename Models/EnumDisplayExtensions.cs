using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HeroQuest.Models;

public static class EnumDisplayExtensions
{
    /// <summary>Devuelve el texto de DisplayAttribute para mostrar enums legibles en las vistas.</summary>
    public static string DisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
    }
}
