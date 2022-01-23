using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models;

public class User
{
    public User(string username, string email, DateTimeOffset dateJoined, byte[] passwordHash)
    {
        Username = username.AssertNotNullOrWhiteSpace();
        Email = email.AssertNotNullOrWhiteSpace();
        DateJoined = dateJoined;
        PasswordHash = passwordHash.AssertNotNull();
    }

    public string Username { get; }

    public string Email { get; }

    public DateTimeOffset DateJoined { get; }

    public byte[] PasswordHash { get; }
}