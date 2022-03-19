namespace TaskManagementSystem.Shared.Models;

public class EventWithParticipants
{
    public EventWithParticipants(EventInfo eventInfo, ICollection<EventParticipantUser> participants)
    {
        EventInfo = eventInfo;
        Participants = participants;
    }

    public EventInfo EventInfo { get; }
    
    public ICollection<EventParticipantUser> Participants { get; }
}