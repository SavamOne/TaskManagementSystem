using TaskManagementSystem.BusinessLogic.Models;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;

namespace TaskManagementSystem.BusinessLogic.Services;

public interface IUserService
{
    Task<User> RegisterUserAsync(RegisterData data);

    Task<User> CheckUserCredentialsAsync(LoginData data);

    Task<User> GetUserAsync(Guid userId);

    Task<User> ChangeUserInfoAsync(ChangeUserInfoData data);

    Task<User> ChangePasswordAsync(ChangePasswordData data);
}