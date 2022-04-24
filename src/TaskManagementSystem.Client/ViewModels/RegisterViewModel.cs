using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.ViewModels;

public class RegisterViewModel
{
	[MinLength(5)]
	public string? Name { get; set; }

	[MinLength(5)]
	public string? Email { get; set; }

	[MinLength(5)]
	public string? Password { get; set; }

	[MinLength(5)]
	[Compare(nameof(PasswordRepeat))]
	public string? PasswordRepeat { get; set; }

	public RegisterRequest GetRequest()
	{
		return new RegisterRequest(Email!, Name!, Password!);
	}
}