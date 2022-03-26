using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories;

public interface ICalendarParticipantRepository
{
    Task<ICollection<CalendarParticipant>> GetByCalendarIdAsync(Guid calendarId);

    Task<CalendarParticipant?> GetByUserAndCalendarId(Guid userId, Guid calendarId);

    Task<ICollection<CalendarParticipant>> GetByIdsAsync(ISet<Guid> ids);

    Task InsertAsync(CalendarParticipant calendar);

    Task InsertAllAsync(ISet<CalendarParticipant> calendarParticipants);

    Task UpdateAllAsync(ISet<CalendarParticipant> calendarParticipants);

    Task DeleteByIdsAsync(ISet<Guid> calendarParticipantsIds);
    
    Task<ICollection<CalendarParticipant>> GetByFilter(Guid calendarId, string filter, int limit);
}