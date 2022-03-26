using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class DeleteParticipantsData
{
	public DeleteParticipantsData(Guid removerId, Guid calendarId, ISet<Guid> participantsIds)
	{
		RemoverId = removerId;
		CalendarId = calendarId;
		ParticipantsIds = participantsIds.AssertNotNull();
	}

	public Guid RemoverId { get; }

	public Guid CalendarId { get; }

	public ISet<Guid> ParticipantsIds { get; }
}