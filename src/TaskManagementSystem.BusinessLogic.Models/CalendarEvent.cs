using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models;

public record CalendarEvent
{
    public CalendarEvent(Guid eventId, string name, string? description, DateTime utcStartTime, DateTime utcEndTime, DateTime utcCreationTime)
    {
        EventId = eventId;
        Name = name.AssertNotNullOrWhiteSpace();
        Description = description;
        UtcStartTime = utcStartTime;
        UtcEndTime = utcEndTime;
        UtcCreationTime = utcCreationTime;
        RecurrentSettings = null;
    }

    public CalendarEvent(Guid eventId, string name, string? description, DateTime utcStartTime, DateTime utcEndTime, DateTime utcCreationTime, RecurrentEventSettings recurrentSettings)
    {
        EventId = eventId;
        Name = name.AssertNotNullOrWhiteSpace();
        Description = description;
        UtcStartTime = utcStartTime;
        UtcEndTime = utcEndTime;
        UtcCreationTime = utcCreationTime;
        RecurrentSettings = recurrentSettings.AssertNotNull();
    }

    public Guid EventId { get; }

    public string Name { get; }

    public string? Description { get; }

    public DateTime UtcStartTime { get; init; }

    public DateTime UtcEndTime { get; init; }

    public DateTime UtcCreationTime { get; }

    public RecurrentEventSettings? RecurrentSettings { get; }
}