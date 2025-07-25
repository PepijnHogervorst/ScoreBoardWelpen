using System.Reflection;

namespace WelpenScoreboard.Domain.Common.Extensions;
/// <summary>
/// Extension class to extend class functionalities
/// </summary>
public static class ClassExtension
{
    /// <summary>
    /// Compares all simple type parameters with given class and current class
    /// </summary>
    public static bool ArePropertiesOfInstancesEqual<T>(this T self, T other, params string[] propertiesToIgnore) where T : class
    {
        if (self == null || other == null) return self == other;

        var type = typeof(T);
        var ignoreList = propertiesToIgnore.ToList();

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => !ignoreList.Contains(p.Name));

        bool areObjectsEqual = false;

        foreach (var property in properties)
        {
            if (!property.GetUnderlyingType()?.IsSimpleType() ?? true) continue;
            if (property.GetIndexParameters().Length != 0) continue;

            object? valueSelf = property.GetValue(self, null);
            object? valueOther = property.GetValue(other, null);

            if (valueSelf != valueOther &&
                (valueSelf == null || !valueSelf.Equals(valueOther))) return false;

            areObjectsEqual = true;
        }
        return areObjectsEqual;
    }

    /// <summary>
    /// Compares all simple type parameters with given class and current class
    /// </summary>
    public static (bool, string) ArePropertiesOfInstancesEqual(this object self, object? other, Type type, bool isEqualIfNoSimpleProperty = false, params string[] propertiesToIgnore)
    {
        if (self == null || other == null) return (self == other, string.Empty);

        var ignoreList = propertiesToIgnore.ToList();

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => !ignoreList.Contains(p.Name));

        bool areObjectsEqual = isEqualIfNoSimpleProperty;

        foreach (var property in properties)
        {
            if (!property.GetUnderlyingType()?.IsSimpleType() ?? true) continue;
            if (property.GetIndexParameters().Length != 0) continue;

            object? valueSelf = property.GetValue(self, null);
            object? valueOther = property.GetValue(other, null);

            if (valueSelf != valueOther &&
                (valueSelf == null || !valueSelf.Equals(valueOther))) return (false, $"'{property.Name}' not equal ('{valueSelf}' ↔ '{valueOther}')");

            areObjectsEqual = true;
        }
        return (areObjectsEqual, string.Empty);
    }
}
