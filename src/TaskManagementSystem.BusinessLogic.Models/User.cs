using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models;

public class User
{
    public string Username { get; }
    
    public string Email { get; }
    
    public DateTimeOffset DateJoined { get; }

    public byte[] PasswordHash { get; }

    public User(string username, string email, DateTimeOffset dateJoined, byte[] passwordHash)
    {
        username.AssertNotNullOrWhiteSpace(nameof(username));
        email.AssertNotNullOrWhiteSpace(nameof(email));
        passwordHash.AssertNotNull(nameof(email));
        
        Username = username;
        Email = email;
        DateJoined = dateJoined;
        PasswordHash = passwordHash;
    }
}