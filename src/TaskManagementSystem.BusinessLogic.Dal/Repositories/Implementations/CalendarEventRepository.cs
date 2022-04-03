using TaskManagementSystem.BusinessLogic.Dal.Converters;
using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Dal;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories.Implementations;

public class CalendarEventRepository : Repository<DalEvent>, ICalendarEventRepository
{
	public CalendarEventRepository(DatabaseConnectionProvider connectionProvider)
		: base(connectionProvider) {}


	public async Task InsertAsync(CalendarEvent calendarEvent)
	{
		await InsertAsync(calendarEvent.ToDalEvent());
	}

	public async Task UpdateAsync(CalendarEvent calendarEvent)
	{
		await UpdateAsync(calendarEvent.ToDalEvent());
	}

	public async Task DeleteByIdAsync(Guid eventId)
	{
		await DeleteMultipleAsync(x => x.Id == eventId);
	}

	public async Task<CalendarEvent?> GetById(Guid eventId)
	{
		DalEvent? dalEvent = await FirstOrDefaultAsync(x => x.Id == eventId);

		return dalEvent?.ToCalendarEvent();
	}

	public async Task<ICollection<CalendarEvent>> GetStandardEventsInRange(Guid calendarId, DateTime startPeriod, DateTime endPeriod)
	{
		var dalEvents = await SelectAsync(x => x.CalendarId == calendarId
											&& x.EndTime >= startPeriod
											&& x.StartTime <= endPeriod);

		return dalEvents.Select(x => x.ToCalendarEvent()).ToList();
	}
	public async Task<ICollection<CalendarEvent>> GetAllStandardEventsWithStartTimeInRange(DateTime startPeriod, DateTime endPeriod)
	{
		var dalEvents = await SelectAsync(x => x.StartTime >= startPeriod && x.StartTime <= endPeriod);
		
		return dalEvents.Select(x => x.ToCalendarEvent()).ToList();
	}
}