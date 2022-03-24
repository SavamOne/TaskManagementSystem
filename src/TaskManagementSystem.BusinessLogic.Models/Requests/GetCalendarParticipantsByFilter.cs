using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class GetCalendarParticipantsByFilter
{
	public GetCalendarParticipantsByFilter(Guid userId, Guid calendarId, string filter)
	{
		CalendarId = calendarId;
		UserId = userId;
		Filter = filter.AssertNotNull();
	}
	
	public Guid UserId { get; }

	public Guid CalendarId { get; }
	
	public string Filter { get; }
}