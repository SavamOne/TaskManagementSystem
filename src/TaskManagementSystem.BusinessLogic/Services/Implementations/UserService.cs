using TaskManagementSystem.BusinessLogic.Helpers;
using TaskManagementSystem.BusinessLogic.Models;

namespace TaskManagementSystem.BusinessLogic.Services.Implementations;

public class UserService : IUserService
{
    private readonly List<User> users = new()
    {
        new User("kek", "aaa@aaa.ru", DateTimeOffset.UtcNow, PasswordHelper.GetHash("hello"))
    };

    public async Task<Result<User>> RegisterUserAsync(RegisterData data)
    {
        //TODO: DataAccess
        User existedUser = users.Find(x => string.Equals(x.Username, data.Username, StringComparison.InvariantCultureIgnoreCase));
        if (existedUser is not null)
        {
            return Result<User>.Error($"User with name {data.Username} already exists");
        }
        
        User user = new(data.Username, data.Email, DateTimeOffset.UtcNow, PasswordHelper.GetHash(data.Password));
        users.Add(user);

        return Result<User>.Success(user);
    }

    public async Task<Result<User>> CheckUserCredentialsAsync(LoginData data)
    {
        //TODO: DataAccess
        User existedUser = users.Find(x => string.Equals(x.Email, data.Email, StringComparison.InvariantCultureIgnoreCase));
        if (existedUser is null)
        {
            return Result<User>.Error("Wrong username or password");
        }

        byte[] loginUserHash = PasswordHelper.GetHash(data.Password);
        if (!loginUserHash.SequenceEqual(existedUser.PasswordHash))
        {
            return Result<User>.Error("Wrong username or password");
        }

        return Result<User>.Success(existedUser);
    }
}