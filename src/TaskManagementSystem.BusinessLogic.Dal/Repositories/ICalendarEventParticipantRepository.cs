using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories;

public interface ICalendarEventParticipantRepository
{
	Task InsertAsync(CalendarEventParticipant participant);

	Task InsertAllAsync(ICollection<CalendarEventParticipant> eventParticipants);

	Task<bool> ContainsCalendarParticipantInEvent(Guid calendarParticipant, Guid eventId);

	Task<CalendarEventParticipant?> GetByUserAndEventId(Guid userId, Guid eventId);

	Task<ICollection<CalendarEventParticipant>> GetByEventId(Guid eventId);

	Task UpdateAllAsync(ICollection<CalendarEventParticipant> eventParticipants);

	Task DeleteByIdsAsync(ISet<Guid> eventParticipantsIds);
}