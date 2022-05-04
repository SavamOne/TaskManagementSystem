using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

/// <summary>
///     Информация о пользователе.
/// </summary>
public class UserInfo
{
	public UserInfo(Guid id,
		string name,
		string email,
		DateTimeOffset dateJoined,
		string? position,
		string? department)
	{
		Id = id;
		Name = name.AssertNotNullOrWhiteSpace();
		Email = email.AssertNotNullOrWhiteSpace();
		DateJoined = dateJoined;
		Position = position;
		Department = department;
	}

	/// <summary>
	///     Id пользователя.
	/// </summary>
	[Required]
	public Guid Id { get; }

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
	
	/// <summary>
	///		Должность.
	/// </summary>
	public string? Position { get; }

	/// <summary>
	///		Отдел.
	/// </summary>
	public string? Department { get; }
	
	/// <summary>
	///     Дата регистрации.
	/// </summary>
	[Required]
	public DateTimeOffset DateJoined { get; }
}