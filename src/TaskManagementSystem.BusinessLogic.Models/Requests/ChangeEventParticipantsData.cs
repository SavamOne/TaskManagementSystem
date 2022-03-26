using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class ChangeEventParticipantsData
{
	public ChangeEventParticipantsData(Guid userId, Guid eventId, ICollection<ChangeEventParticipantData> eventParticipants)
	{
		UserId = userId;
		EventId = eventId;
		EventParticipants = eventParticipants.AssertNotNull();
	}

	public Guid UserId { get;  }

	public Guid EventId { get; }
	
	public ICollection<ChangeEventParticipantData> EventParticipants { get; }
} 