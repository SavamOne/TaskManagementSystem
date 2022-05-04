using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Exceptions;
using TaskManagementSystem.BusinessLogic.Models.Models;

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
			RegisterDate = user.RegisterDateUtc,
			Department = user.Department,
			Position = user.Position
		};
	}

	public static User ToUser(this DalUser user)
	{
		if (user.IsDeleted)
		{
			throw new BusinessLogicException("Этот пользователь удален");
		}

		return new User(user.Id,
			user.Name,
			user.Email,
			user.Position,
			user.Department,
			user.RegisterDate,
			user.PasswordHash);
	}
}