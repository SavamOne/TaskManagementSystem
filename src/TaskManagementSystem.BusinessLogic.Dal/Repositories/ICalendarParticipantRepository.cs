using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories;

public interface ICalendarParticipantRepository
{
    Task<ISet<CalendarParticipant>> GetByCalendarIdAsync(Guid calendarId);

    Task InsertAsync(CalendarParticipant calendar);

    Task InsertAllAsync(ISet<CalendarParticipant> calendarParticipants);

    Task UpdateAllAsync(ISet<CalendarParticipant> calendarParticipants);

    Task DeleteByIds(ISet<Guid> calendarParticipantsIds);
}