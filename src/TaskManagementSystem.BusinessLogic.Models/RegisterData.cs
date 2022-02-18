using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models;

public class RegisterData
{
    public RegisterData(string name, string email, string password)
    {
        Name = name.AssertNotNullOrWhiteSpace();
        Email = email.AssertNotNullOrWhiteSpace();
        Password = password.AssertNotNullOrWhiteSpace();
    }

    public string Name { get; }

    public string Email { get; }

    public string Password { get; }
}