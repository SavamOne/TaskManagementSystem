namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class EditCalendarParticipantsData
{
	public EditCalendarParticipantsData(Guid userId, Guid calendarId, ISet<EditCalendarParticipantData> participants)
	{
		UserId = userId;
		CalendarId = calendarId;
		Participants = participants;
	}

	public Guid UserId { get; }

	public Guid CalendarId { get; }

	public ISet<EditCalendarParticipantData> Participants { get; }
}