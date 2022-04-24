using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories;

public interface ICalendarEventRepository
{
	Task InsertAsync(CalendarEvent calendarEvent);

	Task UpdateAsync(CalendarEvent calendarEvent);

	Task DeleteByIdAsync(Guid eventId);

	Task<CalendarEvent?> GetById(Guid eventId);

	Task<ICollection<CalendarEvent>> GetStandardCalendarEventsInRange(Guid calendarId, DateTime startPeriod, DateTime endPeriod);

	Task<ICollection<CalendarEvent>> GetRepeatedCalendarEvents(Guid calendarId);

	Task<ICollection<CalendarEvent>> GetAllStandardEventsWithStartTimeInRange(DateTime startPeriod, DateTime endPeriod);

	Task<ICollection<CalendarEvent>> GetAllRepeatedEvents();

	Task<ICollection<CalendarEvent>> GetStandardEventsInRangeForUser(Guid userId, DateTime startPeriod, DateTime endPeriod);

	Task<ICollection<CalendarEvent>> GetRepeatedEventsForUser(Guid userId);
}