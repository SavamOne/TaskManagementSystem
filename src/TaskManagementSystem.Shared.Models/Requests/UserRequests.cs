using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Shared.Models.Requests;

/// <summary>
///     Запрос на обновление refresh-токена.
/// </summary>
public class RefreshTokensRequest
{
	public RefreshTokensRequest(string refreshToken)
	{
		RefreshToken = refreshToken;
	}

	/// <summary>
	///     Refresh-токен.
	/// </summary>
	[Required]
	public string RefreshToken { get; }
}

/// <summary>
///     Запрос на авторизацию.
/// </summary>
public class LoginRequest
{
	public LoginRequest(string email, string password)
	{
		Email = email;
		Password = password;
	}

	/// <summary>
	///     Email.
	/// </summary>
	[Required]
	public string Email { get; }

	/// <summary>
	///     Пароль.
	/// </summary>
	[Required]
	public string Password { get; }
}

/// <summary>
///     Запрос на регистрацию.
/// </summary>
public class RegisterRequest
{
	public RegisterRequest(string email, string name, string password)
	{
		Email = email;
		Name = name;
		Password = password;
	}

	/// <summary>
	///     Email.
	/// </summary>
	[Required]
	public string Email { get; }

	/// <summary>
	///     Имя.
	/// </summary>
	[Required]
	public string Name { get; }

	/// <summary>
	///     Пароль.
	/// </summary>
	[Required]
	public string Password { get; }
}

/// <summary>
///     Запрос на смену пароля.
/// </summary>
public class ChangePasswordRequest
{
	public ChangePasswordRequest(string oldPassword, string newPassword)
	{
		OldPassword = oldPassword;
		NewPassword = newPassword;
	}

	/// <summary>
	///     Старый пароль.
	/// </summary>
	[Required]
	public string OldPassword { get; }

	/// <summary>
	///     Новый пароль.
	/// </summary>
	[Required]
	public string NewPassword { get; }
}

/// <summary>
///     Запрос на изменение пользовательской информации.
/// </summary>
public class EditUserInfoRequest
{
	public EditUserInfoRequest(string name, string email)
	{
		Name = name;
		Email = email;
	}

	/// <summary>
	///     Имя.
	/// </summary>
	[Required]
	public string Name { get; }

	/// <summary>
	///     Email.
	/// </summary>
	[Required]
	public string Email { get; }
}

/// <summary>
///     Запрос на получении информации о пользователе.
/// </summary>
public class GetUserInfoByIdRequest
{
	public GetUserInfoByIdRequest(Guid userId)
	{
		UserId = userId;
	}

	/// <summary>
	///     Id пользователя.
	/// </summary>
	[Required]
	public Guid UserId { get; }
}

/// <summary>
///     Запрос на получение пользователей по фильтру.
/// </summary>
public class GetUserInfosByFilterRequest
{
	public GetUserInfosByFilterRequest(string filter)
	{
		Filter = filter;
	}

	/// <summary>
	///     Фильтр email или имени.
	/// </summary>
	[Required]
	public string Filter { get; }
}