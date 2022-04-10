using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Services;

public interface ICalendarEventNotificationService
{
	Task ExecuteAsync(Func<CalendarEventNotification, Task> notificationCallback, CancellationToken token);
}