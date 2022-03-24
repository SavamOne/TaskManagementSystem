using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Models;

public class CalendarEventWithParticipants
{
    public CalendarEventWithParticipants(CalendarEvent @event, IEnumerable<CalendarEventParticipant> participants, bool canUserEditEvent,
        bool canUserEditParticipants)
    {
        CanUserEditEvent = canUserEditEvent;
        CanUserEditParticipants = canUserEditParticipants;
        Event = @event.AssertNotNull();
        Participants = participants.AssertNotNull();
    }

    public bool CanUserEditEvent { get; }
    
    public bool CanUserEditParticipants { get; }
    
    public CalendarEvent Event { get; }
    
    public IEnumerable<CalendarEventParticipant> Participants { get; }
}