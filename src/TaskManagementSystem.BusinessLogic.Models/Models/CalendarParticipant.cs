namespace TaskManagementSystem.BusinessLogic.Models.Models;

public record CalendarParticipant
{
    public CalendarParticipant(Guid id, Guid calendarId, Guid userId, DateTime joinDateUtc, CalendarRole role)
    {
        Id = id;
        CalendarId = calendarId;
        UserId = userId;
        JoinDateUtc = joinDateUtc;
        Role = role;
    }

    public Guid Id { get; }

    public Guid CalendarId { get; }
    
    public Guid UserId { get; }
    
    public DateTime JoinDateUtc { get; }
    
    public CalendarRole Role { get; }
}