using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models;

public class RegisterData
{
    public RegisterData(string username, string email, string password)
    {
        Username = username.AssertNotNullOrWhiteSpace();
        Email = email.AssertNotNullOrWhiteSpace();
        Password = password.AssertNotNullOrWhiteSpace();
    }

    public string Username { get; }

    public string Email { get; }

    public string Password { get; }
}