using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories;

public interface ICalendarEventRepository
{
	Task InsertAsync(CalendarEvent calendarEvent);

	Task UpdateAsync(CalendarEvent calendarEvent);

	Task DeleteByIdAsync(Guid eventId);

	Task<CalendarEvent?> GetById(Guid eventId);

	Task<ICollection<CalendarEvent>> GetStandardEventsInRange(Guid calendarId, DateTime startPeriod, DateTime endPeriod);

	Task<ICollection<CalendarEvent>> GetRepeatedEventsInRange(Guid calendarId);

	Task<ICollection<CalendarEvent>> GetAllStandardEventsWithStartTimeInRange(DateTime startPeriod, DateTime endPeriod);
}