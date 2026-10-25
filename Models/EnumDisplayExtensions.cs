using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HeroQuest.Models;

public static class EnumDisplayExtensions
{
    /// <summary>Devuelve el texto de DisplayAttribute para mostrar enums legibles en las vistas.</summary>
    public static string DisplayName(this Enum value)
    {
        // En enums [Flags], ToString devuelve nombres técnicos; aquí mostramos las etiquetas legibles combinadas.
        if (value.GetType().IsDefined(typeof(FlagsAttribute), inherit: false))
        {
            var selectedNames = Enum.GetValues(value.GetType())
                .Cast<Enum>()
                .Where(flag => Convert.ToInt64(flag) != 0 && value.HasFlag(flag))
                .Select(GetDisplayName)
                .ToArray();

            return selectedNames.Length == 0 ? "Sin especificar" : string.Join(", ", selectedNames);
        }

        return GetDisplayName(value);
    }

    private static string GetDisplayName(Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
    }
}
