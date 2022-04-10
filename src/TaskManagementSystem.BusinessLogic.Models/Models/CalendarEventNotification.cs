namespace TaskManagementSystem.BusinessLogic.Models.Models;

public record CalendarEventNotification(Guid UserId,
	Guid EventId,
	uint RepeatNum,
	DateTime NotificationTimeUtc,
	string EventName)
{
	public (Guid UserId, Guid EventId) Key => ( UserId, EventId );
}