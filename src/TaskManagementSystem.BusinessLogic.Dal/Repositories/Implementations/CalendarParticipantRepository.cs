using Dapper;
using TaskManagementSystem.BusinessLogic.Dal.Converters;
using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories.Implementations;

public class CalendarParticipantRepository : Repository<DalCalendarParticipant>, ICalendarParticipantRepository
{

    public CalendarParticipantRepository(DatabaseConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
    }

    public async Task<ISet<CalendarParticipant>> GetByCalendarIdAsync(Guid calendarId)
    {
        var dalParticipants = await SelectAsync(x => x.CalendarId == calendarId && !x.IsDeleted);

        return dalParticipants.Select(x => x.ToCalendarParticipant()).ToHashSet();
    }

    public async Task InsertAsync(CalendarParticipant calendar)
    {
        calendar.AssertNotNull();
        await InsertAsync(calendar.ToDalCalendarParticipant());
    }
    
    public async Task InsertAllAsync(ISet<CalendarParticipant> calendarParticipants)
    {
        calendarParticipants.AssertNotNull();
        await InsertAllAsync(calendarParticipants.Select(x => x.ToDalCalendarParticipant()));
    }
    
    public async Task UpdateAllAsync(ISet<CalendarParticipant> calendarParticipants)
    {
        calendarParticipants.AssertNotNull();
        foreach (CalendarParticipant calendarParticipant in calendarParticipants)
        {
            await UpdateAsync(calendarParticipant.ToDalCalendarParticipant());
        }
    }
    
    public async Task DeleteByIds(ISet<Guid> calendarParticipantsIds)
    {
        calendarParticipantsIds.AssertNotNull();
        
        await GetConnection().ExecuteScalarAsync("UPDATE calendar_participant SET is_deleted = TRUE WHERE id = ANY(@Ids);", new
        {
            Ids = calendarParticipantsIds.ToList()
        });
    }
}