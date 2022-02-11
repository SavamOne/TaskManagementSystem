using TaskManagementSystem.BusinessLogic.Models;

namespace TaskManagementSystem.BusinessLogic.Services;

public interface IUserService
{
    Task<Result<User>> RegisterUserAsync(RegisterData data);

    Task<Result<User>> CheckUserCredentialsAsync(LoginData data);
    
    Task<Result<User>> GetUserAsync(Guid userId);
    
    Task<Result<User>> ChangeUserInfoAsync(ChangeUserInfoData data);
    
    Task<Result<User>> ChangePasswordAsync(ChangePasswordData data);
}