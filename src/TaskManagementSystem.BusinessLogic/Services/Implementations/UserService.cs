﻿using System.Net.Mail;
using TaskManagementSystem.BusinessLogic.Dal.Repositories;
using TaskManagementSystem.BusinessLogic.Helpers;
using TaskManagementSystem.BusinessLogic.Models.Exceptions;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;
using TaskManagementSystem.BusinessLogic.Resources;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Services.Implementations;

public class UserService : IUserService
{
	private readonly IUserRepository userRepository;

	public UserService(IUserRepository userRepository)
	{
		this.userRepository = userRepository.AssertNotNull();
	}

	public async Task<User> RegisterUserAsync(RegisterData data)
	{
		data.AssertNotNull();

		if (!MailAddress.TryCreate(data.Email, out _))
		{
			throw new BusinessLogicException("Некорректный адрес электронной почты.");
		}

		User? existedUser = await userRepository.GetByEmailAsync(data.Email);
		if (existedUser is not null)
		{
			throw new BusinessLogicException(LocalizedResources.UserAlreadyExists);
		}

		User user = new(Guid.NewGuid(),
			data.Name,
			data.Email,
			null,
			null,
			DateTime.UtcNow,
			PasswordHelper.GetHash(data.Password));

		await userRepository.InsertAsync(user);
		return user;
	}

	public async Task<User> CheckUserCredentialsAsync(LoginData data)
	{
		data.AssertNotNull();

		User? existedUser = await userRepository.GetByEmailAsync(data.Email);
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
		User? user = await userRepository.GetByIdAsync(userId);

		if (user is null)
		{
			throw new BusinessLogicException(LocalizedResources.UserDoesNotExists);
		}

		return user;
	}

	public async Task<ISet<User>> GetUsersAsync(ISet<Guid> userIds)
	{
		userIds.AssertNotNull();

		var result = await userRepository.GetByIdsAsync(userIds);

		if (result.Count != userIds.Count)
		{
			// TODO: Определять конкретный список.
			throw new BusinessLogicException("Найдены не все пользователи");
		}

		return result;
	}

	public async Task<ISet<User>> GetUsersByFilter(string filter)
	{
		filter.AssertNotNull();

		return await userRepository.GetByFilter(filter, 50);
	}

	public async Task<User> EditUserInfoAsync(EditUserInfoData data)
	{
		data.AssertNotNull();

		User? user = await userRepository.GetByIdAsync(data.UserId);

		if (user is null)
		{
			throw new BusinessLogicException(LocalizedResources.UserDoesNotExists);
		}

		string email = user.Email;
		string name = user.Name;
		string? department = user.Department;
		string? position = user.Position;
		if (!string.IsNullOrWhiteSpace(data.Email))
		{
			email = data.Email;
		}
		if (!string.IsNullOrWhiteSpace(data.Name))
		{
			name = data.Name;
		}
		if (!string.IsNullOrWhiteSpace(data.Position))
		{
			position = data.Position;
		}
		if (!string.IsNullOrWhiteSpace(data.Department))
		{
			department = data.Department;
		}

		User updatedUser = new(data.UserId,
			name,
			email,
			position,
			department,
			user.RegisterDateUtc,
			user.PasswordHash);

		await userRepository.UpdateAsync(updatedUser);
		return updatedUser;
	}

	public async Task<User> ChangePasswordAsync(ChangePasswordData data)
	{
		data.AssertNotNull();

		User? user = await userRepository.GetByIdAsync(data.UserId);

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

		User updatedUser = new(data.UserId,
			user.Name,
			user.Email,
			user.Position,
			user.Department,
			user.RegisterDateUtc,
			newHash);

		await userRepository.UpdateAsync(updatedUser);
		return updatedUser;
	}
}