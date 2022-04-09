using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

/// <summary>
///     Участник события.
/// </summary>
public class EventParticipantUser
{
	public EventParticipantUser(string userName,
		string userEmail,
		Guid eventParticipantId,
		Guid calendarParticipantId,
		Guid userId,
		Guid eventId,
		Guid calendarId,
		EventParticipantRole role)
	{
		UserName = userName.AssertNotNull();
		UserEmail = userEmail.AssertNotNull();
		EventParticipantId = eventParticipantId;
		CalendarParticipantId = calendarParticipantId;
		UserId = userId;
		EventId = eventId;
		CalendarId = calendarId;
		Role = role;
	}

	/// <summary>
	///     Имя пользователя.
	/// </summary>
	[Required]
	public string UserName { get; }

	/// <summary>
	///     Email пользователя.
	/// </summary>
	[Required]
	public string UserEmail { get; }

	/// <summary>
	///     Id участника события.
	/// </summary>
	[Required]
	public Guid EventParticipantId { get; }

	/// <summary>
	///     Id участника календаря.
	/// </summary>
	[Required]
	public Guid CalendarParticipantId { get; }

	/// <summary>
	///     Id пользователя.
	/// </summary>
	[Required]
	public Guid UserId { get; }

	/// <summary>
	///     Id события.
	/// </summary>
	[Required]
	public Guid EventId { get; }

	/// <summary>
	///     Id календаря.
	/// </summary>
	[Required]
	public Guid CalendarId { get; }

	/// <summary>
	///     Роль в событии.
	/// </summary>
	[Required]
	public EventParticipantRole Role { get; }
}