using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.ViewModels;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set;  }

    [Required]
    public string Password { get; set; }
    
    public bool RememberMe { get; set; }

    public LoginRequest GetData()
    {
        return new LoginRequest(Email, Password);
    }
}