using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;

namespace TaskManagementSystem.BusinessLogic.Services;

public interface ICalendarEventService
{
    Task<CalendarEvent> CreateEventAsync(AddCalendarEventData data);

    Task DeleteEventAsync(DeleteEventData data);

    Task<CalendarEvent> ChangeEventAsync(ChangeCalendarEventData data);

    Task<CalendarEventWithParticipants> AddEventParticipant(AddEventParticipantsData data);
    
    Task<CalendarEventWithParticipants> ChangeEventParticipants(ChangeEventParticipantsData data);
    
    Task<ICollection<CalendarEvent>> GetEventsInPeriodAsync(GetEventsInPeriodData data);

    Task<CalendarEventWithParticipants> GetEventInfo(GetEventInfoData data);
}