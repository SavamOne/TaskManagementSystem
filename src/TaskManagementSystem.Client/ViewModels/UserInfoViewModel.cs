using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.ViewModels;

public class UserInfoViewModel
{
	public UserInfoViewModel() {}

	public UserInfoViewModel(UserInfo userInfo)
	{
		UserId = userInfo.Id;
		Name = userInfo.Name;
		Email = userInfo.Email;
		RegisterDate = userInfo.DateJoined.ToLocalTime().ToString("d");
	}

	public Guid UserId { get; }

	public string? Name { get; set; }

	public string? Email { get; set; }

	public string? RegisterDate { get; set; }

	public EditUserInfoRequest GetEditInfoRequest()
	{
		return new EditUserInfoRequest(Name!, Email!);
	}
}