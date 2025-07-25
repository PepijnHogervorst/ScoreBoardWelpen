using System.Reflection;

namespace WelpenScoreboard.Domain.Common.Extensions;
/// <summary>
/// Extensions class for <see cref="Type"/>
/// </summary>
public static class TypeExtensions
{
    private static readonly Type[] _primitiveTypes = new[]
    {
        typeof(string),
        typeof(decimal),
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(TimeSpan),
        typeof(Guid)
    };

    /// <summary>
    /// Determine whether a type is simple (String, Decimal, DateTime, etc) 
    /// or complex (i.e. custom class with public properties and methods).
    /// </summary>
    /// <see cref="http://stackoverflow.com/questions/2442534/how-to-test-if-type-is-primitive"/>
    public static bool IsSimpleType(this Type type)
        => type.IsValueType ||
           type.IsPrimitive ||
           _primitiveTypes.Contains(type) ||
           Convert.GetTypeCode(type) != TypeCode.Object;

    /// <summary>
    /// Gets the underlying type of the given member
    /// </summary>
    public static Type? GetUnderlyingType(this MemberInfo member) => member.MemberType switch
    {
        MemberTypes.Event => ((EventInfo)member).EventHandlerType,
        MemberTypes.Field => ((FieldInfo)member).FieldType,
        MemberTypes.Method => ((MethodInfo)member).ReturnType,
        MemberTypes.Property => ((PropertyInfo)member).PropertyType,
        _ => throw new ArgumentException("Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"),
    };
}
