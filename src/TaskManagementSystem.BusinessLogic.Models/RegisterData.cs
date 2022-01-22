using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models;

public class RegisterData
{
    public RegisterData(string username, string email, string password)
    {
        username.AssertNotNullOrWhiteSpace(nameof(username));
        email.AssertNotNullOrWhiteSpace(nameof(email));
        password.AssertNotNullOrWhiteSpace(nameof(password));
        
        Username = username;
        Email = email;
        Password = password;
    }

    public string Username { get; }

    public string Email { get; }

    public string Password { get; }
}