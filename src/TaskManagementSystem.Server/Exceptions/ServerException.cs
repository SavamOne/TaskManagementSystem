using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Server.Exceptions;

public class ServerException : Exception
{
    public ServerException(string description)
    {
        Description = description.AssertNotNullOrWhiteSpace();
    }

    public ServerException(string description, params object?[] arguments)
    {
        description.AssertNotNullOrWhiteSpace();

        Description = string.Format(description, arguments);
    }

    public string Description { get; }

    public override string Message => Description;
}