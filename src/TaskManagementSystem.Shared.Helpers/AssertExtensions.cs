using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TaskManagementSystem.Shared.Helpers;

public static class AssertExtensions
{
    public static string AssertNotNullOrWhiteSpace([NotNull] this string? str, [CallerArgumentExpression("str")] string paramName = "")
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new ArgumentNullException(paramName);
        }

        return str;
    }

    public static T AssertNotNull<T>([NotNull] this T? obj, [CallerArgumentExpression("obj")] string paramName = "")
    {
        if (obj is null)
        {
            throw new ArgumentNullException(paramName);
        }

        return obj;
    }
}