using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;

namespace TaskManagementSystem.BusinessLogic.Services;

public interface ICalendarService
{
    Task<ISet<Calendar>> GetUserCalendars(Guid userId);

    Task<Calendar> CreateCalendarAsync(CreateCalendarData data);

    Task<Calendar> EditCalendarAsync(EditCalendarData data);

    Task<CalendarWithParticipants> AddParticipantsAsync(AddCalendarParticipantsData data);

    Task<CalendarWithParticipants> ChangeParticipantRoleAsync(ChangeParticipantsRoleData data);

    Task<CalendarWithParticipants> DeleteParticipantsAsync(DeleteParticipantsData data);

    Task<CalendarWithParticipants> GetCalendarInfoAsync(Guid id);
}