using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Services.Implementations;

public class UserUpdateService : IUserUpdateService
{
	public void SetUpdatedInfo(UserInfo userInfo)
	{
		UserInfoUpdated?.Invoke(userInfo);
	}

	public event Action<UserInfo>? UserInfoUpdated;
}