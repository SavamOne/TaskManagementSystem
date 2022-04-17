using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Services;

// TODO: Сделать полноценный брокер сообщений. 
public interface IUserUpdateService
{
	public void SetUpdatedInfo(UserInfo userInfo);

	public event Action<UserInfo> UserInfoUpdated;
}