using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class EditEventParticipantsData
{
	public EditEventParticipantsData(Guid userId, Guid eventId, ICollection<EditEventParticipantData> eventParticipants)
	{
		UserId = userId;
		EventId = eventId;
		EventParticipants = eventParticipants.AssertNotNull();
	}

	public Guid UserId { get; }

	public Guid EventId { get; }

	public ICollection<EditEventParticipantData> EventParticipants { get; }
}