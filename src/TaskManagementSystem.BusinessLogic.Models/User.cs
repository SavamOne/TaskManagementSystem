using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models;

public class User
{
    public User(Guid id, string name, string email, DateTimeOffset dateJoined, byte[] passwordHash)
    {
        Name = name.AssertNotNullOrWhiteSpace();
        Email = email.AssertNotNullOrWhiteSpace();
        Id = id;
        DateJoined = dateJoined;
        PasswordHash = passwordHash.AssertNotNull();
    }

    public string Name { get; }

    public string Email { get; }

    public Guid Id { get; }

    public DateTimeOffset DateJoined { get; }

    public byte[] PasswordHash { get; }
}