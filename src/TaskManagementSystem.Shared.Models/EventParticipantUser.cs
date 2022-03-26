using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

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

	public string UserName { get; }

	public string UserEmail { get; }

	public Guid EventParticipantId { get; }

	public Guid CalendarParticipantId { get; }

	public Guid UserId { get; }

	public Guid EventId { get; }

	public Guid CalendarId { get; }

	public EventParticipantRole Role { get; }
}