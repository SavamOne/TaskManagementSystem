using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models;

public class LoginData
{
    public LoginData(string email, string password)
    {
        Email = email.AssertNotNullOrWhiteSpace();
        Password = password.AssertNotNullOrWhiteSpace();
    }

    public string Email { get; }

    public string Password { get; }
}