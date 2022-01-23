using TaskManagementSystem.BusinessLogic.Models;

namespace TaskManagementSystem.BusinessLogic.Services;

public interface IUserService
{
    Task<Result<User>> RegisterUserAsync(RegisterData data);

    Task<Result<User>> CheckUserCredentialsAsync(LoginData data);
}