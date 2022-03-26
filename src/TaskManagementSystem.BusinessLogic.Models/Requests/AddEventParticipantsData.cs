using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class AddEventParticipantsData
{
    public AddEventParticipantsData(Guid userId, Guid eventId, ICollection<AddEventParticipantData> calendarParticipants)
    {
        UserId = userId;
        EventId = eventId;
        CalendarParticipants = calendarParticipants.AssertNotNull();
    }

    public Guid UserId { get; }
    
    public Guid EventId { get; }
    
    public ICollection<AddEventParticipantData> CalendarParticipants { get; }
}

public class AddEventParticipantData
{
    public AddEventParticipantData(Guid calendarParticipantId, CalendarEventParticipantRole role)
    {
        CalendarParticipantId = calendarParticipantId;
        Role = role;
    }

    public Guid CalendarParticipantId { get; }
    
    public CalendarEventParticipantRole Role { get; }
    
}