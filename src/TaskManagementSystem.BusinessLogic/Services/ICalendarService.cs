using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;

namespace TaskManagementSystem.BusinessLogic.Services;

public interface ICalendarService
{
	Task<ICollection<Calendar>> GetUserCalendars(Guid userId);

	Task<Calendar> CreateCalendarAsync(CreateCalendarData data);

	Task<Calendar> EditCalendarAsync(EditCalendarData data);

	Task<CalendarWithParticipants> AddParticipantsAsync(AddCalendarParticipantsData data);

	Task<CalendarWithParticipants> EditParticipantsAsync(ChangeCalendarParticipantsRoleData data);

	Task<CalendarWithParticipants> GetCalendarInfoAsync(Guid id);

	Task<ICollection<CalendarParticipant>> GetParticipantsByFilterAsync(GetCalendarParticipantsByFilter data);
	
	Task<ICollection<CalendarName>> GetCalendarNamesAsync(ISet<Guid> calendarIds);
}