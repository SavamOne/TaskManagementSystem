using Dapper;
using Dommel;
using TaskManagementSystem.BusinessLogic.Dal.Converters;
using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories.Implementations;

public class CalendarEventParticipantRepository : Repository<DalEventParticipant>
{
    public CalendarEventParticipantRepository(DatabaseConnectionProvider connectionProvider)
        : base(connectionProvider) {}

    public async Task InsertAsync(CalendarEventParticipant participant)
    {
        await InsertAsync(participant.ToDalEventParticipant());
    }

    public async Task InsertAllAsync(IEnumerable<CalendarEventParticipant> eventParticipants)
    {
            const string insertSql = "INSERT INTO Event_Participant (Id, Event_Id, Participant_Id, Role, Status_type) "
                                 + "VALUES (@Id, @EventId, @CalendarParticipantId, @Role, 0) "
                                 + "ON CONFLICT (Event_Id, Participant_Id) "
                                 + "DO UPDATE SET Role = @Role, Is_Deleted = FALSE;";

        eventParticipants.AssertNotNull();
        await GetConnection().ExecuteAsync(insertSql, eventParticipants.Select(x => x.ToDalEventParticipant()).ToList());
    }

    public async Task<bool> ContainsCalendarParticipantInEvent(Guid calendarParticipant, Guid eventId)
    {
        return await AnyAsync(x => x.CalendarParticipantId == calendarParticipant && x.EventId == eventId);
    }
    
    public async Task<CalendarEventParticipant?> GetByUserAndEventId(Guid userId, Guid eventId)
    {
        const string getSql = "SELECT * FROM event_participant ep " 
                              + "INNER JOIN calendar_participant cp on cp.id = ep.participant_id " 
                              + "WHERE cp.user_id = @UserId AND ep.event_id = @EventId";

        DalEventParticipant? participant = await GetConnection().QueryFirstOrDefaultAsync<DalEventParticipant>(getSql, new
        {
            userId,
            eventId
        });

        return participant?.ToEventParticipant();
    }

    public async Task<IEnumerable<CalendarEventParticipant>> GetByEventId(Guid eventId)
    {
        var dalParticipants = await SelectAsync(x => x.EventId == eventId);

        return dalParticipants.Select(x => x.ToEventParticipant()).ToList();
    }
}