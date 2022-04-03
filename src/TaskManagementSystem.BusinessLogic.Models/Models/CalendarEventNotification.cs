namespace TaskManagementSystem.BusinessLogic.Models.Models;

public record CalendarEventNotification(Guid UserId, Guid EventId, DateTime NotificationTimeUtc, string EventName)
{
	public (Guid UserId, Guid EventId) Key => ( UserId, EventId );
}