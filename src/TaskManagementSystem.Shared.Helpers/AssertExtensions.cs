namespace TaskManagementSystem.Shared.Helpers;

public static class AssertExtensions
{
    public static void AssertNotNullOrWhiteSpace(this string str, string paramName)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new ArgumentNullException(paramName);
        }
    }
    
    public static void AssertNotNull(this object obj, string paramName)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}