using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

/// <summary>
///     Участник календаря.
/// </summary>
public class CalendarParticipantUser
{
	public CalendarParticipantUser(Guid id,
		Guid calendarId,
		Guid userId,
		DateTimeOffset calendarJoinDate,
		CalendarParticipantRole role,
		string username,
		string email,
		DateTimeOffset registerDate)
	{
		Username = username.AssertNotNullOrWhiteSpace();
		Email = email.AssertNotNullOrWhiteSpace();

		Id = id;
		CalendarId = calendarId;
		UserId = userId;
		CalendarJoinDate = calendarJoinDate;
		Role = role;
		RegisterDate = registerDate;
	}

	/// <summary>
	///     Id участника события.
	/// </summary>
	[Required]
	public Guid Id { get; }

	/// <summary>
	///     Id календаря.
	/// </summary>
	[Required]
	public Guid CalendarId { get; }

	/// <summary>
	///     Id пользователя
	/// </summary>
	[Required]
	public Guid UserId { get; }

	/// <summary>
	///     Дата присоединения в календарь.
	/// </summary>
	[Required]
	public DateTimeOffset CalendarJoinDate { get; }

	/// <summary>
	///     Роль в календаре.
	/// </summary>
	[Required]
	public CalendarParticipantRole Role { get; }

	/// <summary>
	///     Имя пользователя.
	/// </summary>
	[Required]
	public string Username { get; }

	/// <summary>
	///     Email.
	/// </summary>
	[Required]
	public string Email { get; }

	/// <summary>
	///     Дата регистрации.
	/// </summary>
	[Required]
	public DateTimeOffset RegisterDate { get; }
}