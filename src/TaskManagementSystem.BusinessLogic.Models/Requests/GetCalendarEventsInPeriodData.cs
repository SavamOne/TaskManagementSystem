namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class GetCalendarEventsInPeriodData
{
	public GetCalendarEventsInPeriodData(Guid userId,
		Guid calendarId,
		DateTimeOffset startPeriod,
		DateTimeOffset endPeriod)
	{
		UserId = userId;
		CalendarId = calendarId;
		StartPeriod = startPeriod;
		EndPeriod = endPeriod;
	}

	public Guid UserId { get; }

	public Guid CalendarId { get; }

	public DateTimeOffset StartPeriod { get; }

	public DateTimeOffset EndPeriod { get; }
}