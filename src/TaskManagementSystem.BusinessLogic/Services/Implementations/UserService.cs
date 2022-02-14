using TaskManagementSystem.BusinessLogic.Exceptions;
using TaskManagementSystem.BusinessLogic.Helpers;
using TaskManagementSystem.BusinessLogic.Models;
using TaskManagementSystem.BusinessLogic.Resources;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Services.Implementations;

public class UserService : IUserService
{
    private readonly List<User> users = new()
    {
        new User(new Guid("8B51CF94-CD5D-48BD-8B40-54539CD9212D"), "Anton Sav", "aaa@aaa.ru", DateTimeOffset.UtcNow, PasswordHelper.GetHash("hello"))
    };

    public async Task<User> RegisterUserAsync(RegisterData data)
    {
        data.AssertNotNull();

        //TODO: DataAccess
        User? existedUser = users.Find(x =>
            string.Equals(x.Email, data.Email, StringComparison.InvariantCultureIgnoreCase));
        if (existedUser is not null)
        {
            throw new BusinessLogicException(LocalizedResources.UserAlreadyExists);
        }

        User user = new(Guid.NewGuid(), data.Username, data.Email, DateTimeOffset.UtcNow, PasswordHelper.GetHash(data.Password));
        users.Add(user);

        return user;
    }

    public async Task<User> CheckUserCredentialsAsync(LoginData data)
    {
        Console.WriteLine(Thread.CurrentThread.CurrentUICulture.Name);
        data.AssertNotNull();

        //TODO: DataAccess
        User? existedUser =
            users.Find(x => string.Equals(x.Email, data.Email, StringComparison.InvariantCultureIgnoreCase));
        if (existedUser is null)
        {
            throw new BusinessLogicException(LocalizedResources.WrongEmailOrPassword);
        }

        byte[] loginUserHash = PasswordHelper.GetHash(data.Password);
        if (!loginUserHash.SequenceEqual(existedUser.PasswordHash))
        {
            throw new BusinessLogicException(LocalizedResources.WrongEmailOrPassword);
        }

        return existedUser;
    }

    public async Task<User> GetUserAsync(Guid userId)
    {
        User? user = users.FirstOrDefault(x => x.Id == userId);

        if (user is null)
        {
            throw new BusinessLogicException(LocalizedResources.UserDoesNotExists);
        }

        return user;
    }
    public async Task<User> ChangeUserInfoAsync(ChangeUserInfoData data)
    {
        data.AssertNotNull();

        User? user = users.FirstOrDefault(x => x.Id == data.UserId);

        if (user is null)
        {
            throw new BusinessLogicException(LocalizedResources.UserDoesNotExists);
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

        return updatedUser;
    }
    public async Task<User> ChangePasswordAsync(ChangePasswordData data)
    {
        data.AssertNotNull();

        User? user = users.FirstOrDefault(x => x.Id == data.UserId);

        if (user is null)
        {
            throw new BusinessLogicException(LocalizedResources.UserDoesNotExists);
        }

        byte[] oldHash = PasswordHelper.GetHash(data.OldPassword);
        if (!oldHash.SequenceEqual(user.PasswordHash))
        {
            throw new BusinessLogicException(LocalizedResources.OldPasswordIsIncorrect);
        }

        byte[] newHash = PasswordHelper.GetHash(data.NewPassword);
        if (newHash.SequenceEqual(oldHash))
        {
            throw new BusinessLogicException(LocalizedResources.NewPasswordIsTheSame);
        }

        User updatedUser = new(data.UserId, user.Name, user.Email, user.DateJoined, newHash);
        users.Add(updatedUser);
        users.Remove(user);

        return updatedUser;
    }
}