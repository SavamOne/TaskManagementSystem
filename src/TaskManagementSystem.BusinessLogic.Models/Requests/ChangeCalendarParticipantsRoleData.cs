namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class ChangeCalendarParticipantsRoleData
{
	public ChangeCalendarParticipantsRoleData(Guid userId, Guid calendarId, ISet<ChangeCalendarParticipantRoleData> participants)
	{
		UserId = userId;
		CalendarId = calendarId;
		Participants = participants;
	}

	public Guid UserId { get; }

	public Guid CalendarId { get; }

	public ISet<ChangeCalendarParticipantRoleData> Participants { get; }
}