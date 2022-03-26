using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public class EventWithParticipants
{
	public EventWithParticipants(EventInfo eventInfo,
		ICollection<EventParticipantUser> participants,
		bool canUserEditEvent,
		bool canUserEditParticipants,
		bool canUserDeleteEvent)
	{
		EventInfo = eventInfo.AssertNotNull();
		Participants = participants.AssertNotNull();
		CanUserEditEvent = canUserEditEvent;
		CanUserEditParticipants = canUserEditParticipants;
		CanUserDeleteEvent = canUserDeleteEvent;
	}

	public bool CanUserEditEvent { get; }

	public bool CanUserEditParticipants { get; }

	public bool CanUserDeleteEvent { get; }

	public EventInfo EventInfo { get; }

	public ICollection<EventParticipantUser> Participants { get; }
}