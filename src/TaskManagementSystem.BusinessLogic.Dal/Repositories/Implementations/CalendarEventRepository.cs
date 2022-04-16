using Dapper;
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

	public async Task<ICollection<CalendarEvent>> GetStandardCalendarEventsInRange(Guid calendarId, DateTime startPeriod, DateTime endPeriod)
	{
		var dalEvents = await SelectAsync(x => x.CalendarId == calendarId
											&& x.EndTime >= startPeriod
											&& x.StartTime <= endPeriod 
											&& !x.IsRepeated);

		return dalEvents.Select(x => x.ToCalendarEvent()).ToList();
	}

	public async Task<ICollection<CalendarEvent>> GetRepeatedCalendarEvents(Guid calendarId)
	{
		var dalEvents = await SelectAsync(x => x.CalendarId == calendarId && x.IsRepeated);

		return dalEvents.Select(x => x.ToCalendarEvent()).ToList();
	}

	public async Task<ICollection<CalendarEvent>> GetAllStandardEventsWithStartTimeInRange(DateTime startPeriod, DateTime endPeriod)
	{
		var dalEvents = await SelectAsync(x => x.StartTime >= startPeriod && x.StartTime <= endPeriod && !x.IsRepeated);

		return dalEvents.Select(x => x.ToCalendarEvent()).ToList();
	}
	
	public async Task<ICollection<CalendarEvent>> GetAllRepeatedEvents()
	{
		var dalEvents = await SelectAsync(x => x.IsRepeated);

		return dalEvents.Select(x => x.ToCalendarEvent()).ToList();
	}
	
	public async Task<ICollection<CalendarEvent>> GetStandardEventsInRangeForUser(Guid userId, DateTime startPeriod, DateTime endPeriod)
	{
		const string selectSql = "SELECT * FROM event e " 
						 + "INNER JOIN event_participant ep on e.id = ep.event_id " 
						+ "INNER JOIN calendar_participant cp on ep.calendar_participant_id = cp.id "
						 + "WHERE cp.user_id = @UserId and e.is_repeated = FALSE " 
						 + "AND e.end_time >= @StartPeriod AND e.start_time <= @EndPeriod";

		var dalEvents = await GetConnection()
		   .QueryAsync<DalEvent>(selectSql,
				new
				{
					UserId = userId,
					StartPeriod = startPeriod,
					EndPeriod = endPeriod
				});

		return dalEvents.Select(x => x.ToCalendarEvent()).ToList();
	}
	
	public async Task<ICollection<CalendarEvent>> GetRepeatedEventsForUser(Guid userId)
	{
		const string selectSql = "SELECT * FROM event e "
							   + "INNER JOIN event_participant ep on e.id = ep.event_id "
							   + "INNER JOIN calendar_participant cp on ep.calendar_participant_id = cp.id "
							   + "WHERE cp.user_id = @UserId and e.is_repeated = TRUE ";

		var dalEvents = await GetConnection()
		   .QueryAsync<DalEvent>(selectSql,
				new
				{
					UserId = userId,
				});

		return dalEvents.Select(x => x.ToCalendarEvent()).ToList();
	}
}