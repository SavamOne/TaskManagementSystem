using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public class UserInfo
{
    public UserInfo(string name, string email, DateTimeOffset dateJoined)
    {
        Name = name.AssertNotNullOrWhiteSpace();
        Email = email.AssertNotNullOrWhiteSpace();
        DateJoined = dateJoined;
    }

    public string Name { get; }

    public string Email { get; }

    public DateTimeOffset DateJoined { get; }
}