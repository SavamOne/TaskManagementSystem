using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Models;

public class CalendarEventWithParticipants
{
    public CalendarEventWithParticipants(CalendarEvent @event, IEnumerable<CalendarEventParticipant> participants)
    {
        Event = @event.AssertNotNull();
        Participants = participants.AssertNotNull();
    }

    public CalendarEvent Event { get; }
    
    public IEnumerable<CalendarEventParticipant> Participants { get; }
}