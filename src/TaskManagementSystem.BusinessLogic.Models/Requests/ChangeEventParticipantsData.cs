using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class ChangeEventParticipantsData
{
	public ChangeEventParticipantsData(Guid userId, Guid eventId, ICollection<ChangeEventParticipantData> participants)
	{
		UserId = userId;
		EventId = eventId;
		Participants = participants;
	}

	public Guid UserId { get;  }

	public Guid EventId { get; }
	
	public ICollection<ChangeEventParticipantData> Participants { get; }
} 