using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class ChangePasswordData
{
	public ChangePasswordData(Guid userId, string oldPassword, string newPassword)
	{
		UserId = userId;
		OldPassword = oldPassword.AssertNotNullOrWhiteSpace();
		NewPassword = newPassword.AssertNotNullOrWhiteSpace();
	}

	public Guid UserId { get; }

	public string OldPassword { get; }

	public string NewPassword { get; }
}