using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CareSync.Domain.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
        => ((Enum)(object)enumValue).GetDisplayName();

    public static string GetDisplayName(this Enum enumValue)
    {
        var type = enumValue.GetType();
        var name = enumValue.ToString();
        var member = type.GetMember(name).FirstOrDefault();
        if (member != null)
        {
            var display = member.GetCustomAttribute<DisplayAttribute>();
            if (display != null && !string.IsNullOrWhiteSpace(display.Name))
                return display.Name;
        }

        // Fallback: split PascalCase into words
        return System.Text.RegularExpressions.Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
    }
}
