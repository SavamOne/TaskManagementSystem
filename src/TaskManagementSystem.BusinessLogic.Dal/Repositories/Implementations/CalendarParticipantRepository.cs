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
        const string insertSql = "INSERT INTO Calendar_Participant (Id, Calendar_Id, User_Id, Role, Join_Date) "
        + "VALUES (@Id, @CalendarId, @UserId, @Role, @JoinDate) " 
        + "ON CONFLICT (Calendar_Id, User_Id) " 
        + "DO UPDATE SET Role = @Role, Join_Date = @JoinDate, Is_Deleted = FALSE;";
        
        calendarParticipants.AssertNotNull();
        await GetConnection().ExecuteAsync(insertSql, calendarParticipants.Select(x=> x.ToDalCalendarParticipant()).ToArray());
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
        const string deleteSql = "UPDATE calendar_participant SET is_deleted = TRUE WHERE id = ANY(@Ids);";
        
        calendarParticipantsIds.AssertNotNull();
        
        await GetConnection().ExecuteScalarAsync(deleteSql, new
        {
            Ids = calendarParticipantsIds.ToList()
        });
    }
}