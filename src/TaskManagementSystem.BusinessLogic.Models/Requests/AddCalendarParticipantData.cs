using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public record AddCalendarParticipantData
{
    public AddCalendarParticipantData(Guid userId, CalendarRole role)
    {
        UserId = userId;
        Role = role;
    }

    public Guid UserId { get; }
    
    public CalendarRole Role { get; }
}