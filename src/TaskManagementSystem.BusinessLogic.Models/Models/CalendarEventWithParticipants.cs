using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Models;

public class CalendarEventWithParticipants
{
	public CalendarEventWithParticipants(CalendarEvent @event,
		IEnumerable<CalendarEventParticipant> participants,
		bool canUserEditEvent,
		bool canUserEditParticipants,
		bool canUserDeleteEvent)
	{
		CanUserEditEvent = canUserEditEvent;
		CanUserEditParticipants = canUserEditParticipants;
		CanUserDeleteEvent = canUserDeleteEvent;
		Event = @event.AssertNotNull();
		Participants = participants.AssertNotNull();
	}

	public bool CanUserEditEvent { get; }

	public bool CanUserEditParticipants { get; }

	public bool CanUserDeleteEvent { get; }

	public CalendarEvent Event { get; }

	public IEnumerable<CalendarEventParticipant> Participants { get; }
}