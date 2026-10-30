using System.ComponentModel.DataAnnotations;
using System.Reflection; // Para usar GetCustomAttribute<DisplayAttribute>.

namespace HeroQuest.Models;

public static class EnumDisplayExtensions // <<static>> indica que la clase no puede crear objetos con <<new>> y que todos sus métodos deben ser estáticos.
{
    public static string DisplayName(this Enum value) // <<this>> indica que es un método de extensión, lo que permite llamar a este método directamente sobre una instancia de enum.
    {
        /// <summary>Solo se ejecutará en caso de que el enum tenga el atributo [Flags].</summary>        
        if (value.GetType().IsDefined(typeof(FlagsAttribute), inherit: false)) //<<value.GetType().IsDefined(typeof(FlagsAttribute)>> esta preguntando si el <<type>> de <<value>> esta <<definido>> como <<FlagsAttribute>>. <<inherit: false>> indica que no se hereda de clases base de tipo enum.
        {
            var selectedNames = Enum.GetValues(value.GetType()) // Devuelve un array de todos los valores del enum. En este caso del enum TypeAppears.
                .Cast<Enum>() // Hace casting de cada valor del array a Enum.
                .Where(flag => Convert.ToInt64(flag) != 0 && value.HasFlag(flag)) // Filtra los valores del enum. <<Convert.ToInt64(flag) != 0>> descarta el valor None cuyo valor es 0. <<value.HasFlag(flag)>> comprueba si el valor del enum actual tiene el flag correspondiente.
                .Select(GetDisplayName) // Proyecta cada valor del enum filtrado a su nombre de visualización usando el método GetDisplayName.
                .ToArray();

            return selectedNames.Length == 0 ? "Sin especificar" : string.Join(", ", selectedNames); // Si no hay valores seleccionados, devuelve "Sin especificar". De lo contrario, une los nombres de visualización con comas y los devuelve como una cadena.
        }

        return GetDisplayName(value); // Si el enum no tiene el atributo [Flags], simplemente devuelve el nombre de visualización del valor del enum usando el método GetDisplayName.
    }

    /// <summary>Devuelve el texto de que ha sido configurado en el atributo [Display].</summary>
    private static string GetDisplayName(Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault(); // Obtiene el miembro del enum correspondiente al valor actual. <<value.ToString()>> convierte el valor del enum a su nombre de cadena. <<FirstOrDefault()>> devuelve el primer miembro encontrado o null si no se encuentra ninguno.
        return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString(); // <<member?>> evita errores si este es null. <<GetCustomAttribute<DisplayAttribute>()>> comprueba el atributo [Display] del miembro. <<GetName()>> obtiene el <<Name>> configurado en el atributo [Display]. <<?? value.ToString()>> si no se encuentra el atributo [Display], devuelve el nombre del enum como cadena.
    }
}

/*Con "this": método de extensión.
Se llama directamente sobre el enum.
TypeAppears aparece = TypeAppears.JuegoBase;

string texto = aparece.DisplayName(); 


Sin "this": método estático normal.
Hay que llamar a la clase y pasarle el enum como parámetro.
TypeAppears aparece = TypeAppears.JuegoBase;

string texto = EnumDisplayExtensions.DisplayName(aparece);*/