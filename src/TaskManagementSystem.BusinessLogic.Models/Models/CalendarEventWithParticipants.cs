using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Models;

public class CalendarEventWithParticipants
{
	public CalendarEventWithParticipants(CalendarEvent @event,
		IEnumerable<CalendarEventParticipant> participants,
		bool canUserEditEvent,
		bool canUserEditParticipants,
		bool canUserDeleteEvent,
		CalendarEventParticipantState? participationState,
		TimeSpan? notifyBefore,
		RecurrentEventSettings? recurrentEventSettings)
	{
		CanUserEditEvent = canUserEditEvent;
		CanUserEditParticipants = canUserEditParticipants;
		CanUserDeleteEvent = canUserDeleteEvent;
		RecurrentEventSettings = recurrentEventSettings;
		ParticipationState = participationState;
		NotifyBefore = notifyBefore;
		Event = @event.AssertNotNull();
		Participants = participants.AssertNotNull();
	}

	public bool CanUserEditEvent { get; }

	public bool CanUserEditParticipants { get; }

	public bool CanUserDeleteEvent { get; }
	
	public CalendarEventParticipantState? ParticipationState { get; }

	public TimeSpan? NotifyBefore { get; }
	
	public CalendarEvent Event { get; }

	public IEnumerable<CalendarEventParticipant> Participants { get; }
	
	public RecurrentEventSettings? RecurrentEventSettings { get; }
}