namespace TaskManagementSystem.Shared.Models;

public class EventParticipantUser
{
    public EventParticipantUser(Guid id, Guid eventId, Guid calendarParticipantId, EventParticipantRole role)
    {
        Id = id;
        EventId = eventId;
        CalendarParticipantId = calendarParticipantId;
        Role = role;
    }

    public Guid Id { get; }
    
    public Guid EventId { get; }
    
    public Guid CalendarParticipantId { get; }
    
    public EventParticipantRole Role { get; }
}