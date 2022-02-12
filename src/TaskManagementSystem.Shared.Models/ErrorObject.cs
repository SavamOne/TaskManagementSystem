using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public struct ErrorObject
{
    public ErrorObject(string error)
    {
        Error = error.AssertNotNull();
    }

    public string Error { get; }
}