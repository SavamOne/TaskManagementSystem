using Dapper;
using Dommel;
using TaskManagementSystem.BusinessLogic.Dal.Converters;
using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models;
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
    
    public async Task<CalendarParticipant?> GetByIdAsync(Guid id)
    {
        DalUser? dalUser = null;
        DalCalendarParticipant? dalCalendar = await GetConnection().FirstOrDefaultAsync<DalCalendarParticipant, DalUser, DalCalendarParticipant>(participant => participant.Id == id, 
        (participant, user) =>
        {
            dalUser = user;
            return participant;
        });

        return dalCalendar?.ToCalendarParticipant();
    }

    public async Task<ISet<CalendarParticipant>> GetByCalendarIdAsync(Guid calendarId)
    {
        var dalParticipants = await SelectAsync(x => x.CalendarId == calendarId);

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
    
    public async Task DeleteByIds(ISet<Guid> calendarParticipantsIds)
    {
        calendarParticipantsIds.AssertNotNull();

        await GetConnection().ExecuteAsync("UPDATE calendar_participant SET is_deleted = TRUE WHERE id IN @Ids", new
        {
            Ids = calendarParticipantsIds
        });
    }
}