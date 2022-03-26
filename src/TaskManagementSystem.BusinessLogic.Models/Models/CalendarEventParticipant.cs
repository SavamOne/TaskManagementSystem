namespace TaskManagementSystem.BusinessLogic.Models.Models;

public class CalendarEventParticipant
{
	public CalendarEventParticipant(Guid id,
		Guid eventId,
		Guid calendarParticipantId,
		CalendarEventParticipantRole role)
	{
		Id = id;
		EventId = eventId;
		CalendarParticipantId = calendarParticipantId;
		Role = role;
	}

	public Guid Id { get; }

	public Guid EventId { get; }

	public Guid CalendarParticipantId { get; }

	public CalendarEventParticipantRole Role { get; set; }

	public CalendarParticipant? CalendarParticipant { get; init; }
}