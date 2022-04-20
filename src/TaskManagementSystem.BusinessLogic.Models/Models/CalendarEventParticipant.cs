namespace TaskManagementSystem.BusinessLogic.Models.Models;

public class CalendarEventParticipant
{
	public CalendarEventParticipant(Guid id,
		Guid eventId,
		Guid calendarParticipantId,
		CalendarEventParticipantRole role,
		CalendarEventParticipantState state)
	{
		Id = id;
		EventId = eventId;
		CalendarParticipantId = calendarParticipantId;
		Role = role;
		State = state;
	}

	public Guid Id { get; }

	public Guid EventId { get; }

	public Guid CalendarParticipantId { get; }

	public CalendarEventParticipantState State { get; set; }

	public TimeSpan NotifyBefore { get; set; } = TimeSpan.Zero;

	public CalendarEventParticipantRole Role { get; set; }

	public CalendarParticipant? CalendarParticipant { get; init; }
}