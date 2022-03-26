namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class DeleteEventData
{
    public DeleteEventData(Guid userId, Guid eventId)
    {
        UserId = userId;
        EventId = eventId;
    }
    
    public Guid UserId { get; }
    
    public Guid EventId { get; }
}