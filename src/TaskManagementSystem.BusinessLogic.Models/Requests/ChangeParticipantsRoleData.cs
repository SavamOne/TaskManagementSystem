namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class ChangeParticipantsRoleData
{
	public ChangeParticipantsRoleData(Guid changerId, Guid calendarId, ISet<ChangeParticipantRoleData> participants)
	{
		ChangerId = changerId;
		CalendarId = calendarId;
		Participants = participants;
	}

	public Guid ChangerId { get; }

	public Guid CalendarId { get; }

	public ISet<ChangeParticipantRoleData> Participants { get; }
}