using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class EditParticipationStateData
{
	public EditParticipationStateData(Guid userId,
		Guid eventId,
		CalendarEventParticipantState state,
		TimeSpan? notifyBefore)
	{
		UserId = userId;
		State = state;
		NotifyBefore = notifyBefore;
		EventId = eventId;
	}

	public Guid UserId { get; }

	public Guid EventId { get; }

	public CalendarEventParticipantState State { get; }

	public TimeSpan? NotifyBefore { get; }
}