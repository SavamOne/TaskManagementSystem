using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.Converters;

public static class UserConverter
{
    public static DalUser ToDalUser(this User user)
    {
        return new DalUser
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            IsDeleted = false,
            PasswordHash = user.PasswordHash,
            RegisterDate = user.DateJoinedUtc
        };
    }

    public static User ToUser(this DalUser user)
    {
        return new User(user.Id, user.Name, user.Email, user.RegisterDate, user.PasswordHash);
    }
}