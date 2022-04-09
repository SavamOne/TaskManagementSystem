using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.ViewModels;

public class LoginViewModel
{
	[Required]
	[EmailAddress]
	public string? Email { get; set; }

	[Required]
	public string? Password { get; set; }

	public LoginRequest GetData()
	{
		return new LoginRequest(Email!, Password!);
	}
}