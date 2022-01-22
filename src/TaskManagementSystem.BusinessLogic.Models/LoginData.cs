using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models;

public class LoginData
{
    public LoginData(string email, string password)
    {
        email.AssertNotNullOrWhiteSpace(nameof(email));
        password.AssertNotNullOrWhiteSpace(nameof(password));
        
        Email = email;
        Password = password;
    }

    public string Email { get; }

    public string Password { get; }
}