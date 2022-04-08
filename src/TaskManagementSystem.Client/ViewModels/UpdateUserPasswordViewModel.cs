using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.ViewModels;

public class UpdateUserPasswordViewModel
{
	[MinLength(5)]
	public string? OldPassword { get; set; }

	[MinLength(5)]
	public string? NewPassword { get; set; }

	[MinLength(5)]
	[Compare(nameof(NewPassword))]
	public string? NewPasswordRepeat { get; set; }

	public ChangePasswordRequest GetRequest()
	{
		return new ChangePasswordRequest(OldPassword!, NewPassword!);
	}
}