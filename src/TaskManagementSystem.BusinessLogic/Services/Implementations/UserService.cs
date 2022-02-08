using TaskManagementSystem.BusinessLogic.Helpers;
using TaskManagementSystem.BusinessLogic.Models;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Services.Implementations;

public class UserService : IUserService
{
    private readonly List<User> users = new()
    {
        new User(new Guid("8B51CF94-CD5D-48BD-8B40-54539CD9212D"), "Anton Sav", "aaa@aaa.ru", DateTimeOffset.UtcNow, PasswordHelper.GetHash("hello"))
    };

    public async Task<Result<User>> RegisterUserAsync(RegisterData data)
    {
        data.AssertNotNull();

        //TODO: DataAccess
        User? existedUser = users.Find(x =>
            string.Equals(x.Name, data.Username, StringComparison.InvariantCultureIgnoreCase));
        if (existedUser is not null)
        {
            return Result<User>.Error($"User with name {data.Username} already exists");
        }

        User user = new(Guid.NewGuid(), data.Username, data.Email, DateTimeOffset.UtcNow, PasswordHelper.GetHash(data.Password));
        users.Add(user);

        return Result<User>.Success(user);
    }

    public async Task<Result<User>> CheckUserCredentialsAsync(LoginData data)
    {
        data.AssertNotNull();

        //TODO: DataAccess
        User? existedUser =
            users.Find(x => string.Equals(x.Email, data.Email, StringComparison.InvariantCultureIgnoreCase));
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
    
    public async Task<Result<User>> GetUserAsync(Guid userId)
    {
        User? user = users.FirstOrDefault(x => x.Id == userId);

        if (user == null)
        {
            return Result<User>.Error("User does not exists");
        }
        
        return Result<User>.Success(user);
    }
    public async Task<Result<User>> ChangeUserInfoAsync(ChangeUserInfoData data)
    {
        data.AssertNotNull();
        
        User? user = users.FirstOrDefault(x => x.Id == data.UserId);

        if (user == null)
        {
            return Result<User>.Error("User does not exists");
        }

        string email = user.Email;
        string name = user.Name;
        if (!string.IsNullOrWhiteSpace(data.Email))
        {
            email = data.Email;
        }
        if (!string.IsNullOrWhiteSpace(data.Name))
        {
            name = data.Name;
        }

        User updatedUser = new(data.UserId, name, email, user.DateJoined, user.PasswordHash);
        users.Add(updatedUser);
        users.Remove(user);

        return Result<User>.Success(updatedUser);
    }
    public async Task<Result<User>> ChangePasswordAsync(ChangePasswordData data)
    {
        data.AssertNotNull();
        
        User? user = users.FirstOrDefault(x => x.Id == data.UserId);

        if (user == null)
        {
            return Result<User>.Error("User does not exists");
        }

        byte[] oldHash = PasswordHelper.GetHash(data.OldPassword);
        if (!oldHash.SequenceEqual(user.PasswordHash))
        {
            return Result<User>.Error("Old password is incorrect");
        }
        
        byte[] newHash = PasswordHelper.GetHash(data.NewPassword);
        if (newHash.SequenceEqual(oldHash))
        {
            return Result<User>.Error("New password is the same"); 
        }
        
        User updatedUser = new(data.UserId, user.Name, user.Email, user.DateJoined, newHash);
        users.Add(updatedUser);
        users.Remove(user);

        return Result<User>.Success(updatedUser);
    }
}