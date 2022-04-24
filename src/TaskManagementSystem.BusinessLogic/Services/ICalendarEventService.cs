using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;

namespace TaskManagementSystem.BusinessLogic.Services;

public interface ICalendarEventService
{
	Task<CalendarEvent> CreateEventAsync(AddCalendarEventData data);

	Task DeleteEventAsync(DeleteEventData data);

	Task<CalendarEvent> EditEventAsync(EditCalendarEventData data);

	Task<CalendarEventWithParticipants> AddEventParticipant(AddEventParticipantsData data);

	Task<CalendarEventWithParticipants> EditEventParticipants(EditEventParticipantsData data);

	Task<ICollection<CalendarEvent>> GetCalendarEventsInPeriodAsync(GetCalendarEventsInPeriodData data);

	Task<ICollection<CalendarEvent>> GetEventsForUserInPeriodAsync(GetEventsInPeriodForUserData data);

	Task<CalendarEventWithParticipants> GetEventInfo(GetEventInfoData data);

	Task<CalendarEventWithParticipants> EditParticipationState(EditParticipationStateData data);
}