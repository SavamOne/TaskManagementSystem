namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class GetEventsInPeriodForUserData
{
	public GetEventsInPeriodForUserData(Guid userId,
		DateTimeOffset startPeriod,
		DateTimeOffset endPeriod)
	{
		UserId = userId;
		StartPeriod = startPeriod;
		EndPeriod = endPeriod;
	}

	public Guid UserId { get; }

	public DateTimeOffset StartPeriod { get; }

	public DateTimeOffset EndPeriod { get; }
}