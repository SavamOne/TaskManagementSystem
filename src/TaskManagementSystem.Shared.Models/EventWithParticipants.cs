namespace TaskManagementSystem.Shared.Models;

public class EventWithParticipants
{
    public EventWithParticipants(EventInfo eventInfo, ICollection<EventParticipantUser> participants, bool canUserEditEvent,
        bool canUserEditParticipants)
    {
        EventInfo = eventInfo;
        Participants = participants;
        CanUserEditEvent = canUserEditEvent;
        CanUserEditParticipants = canUserEditParticipants;
    }

    public bool CanUserEditEvent { get; }
    
    public bool CanUserEditParticipants { get; }
    
    public EventInfo EventInfo { get; }
    
    public ICollection<EventParticipantUser> Participants { get; }
}