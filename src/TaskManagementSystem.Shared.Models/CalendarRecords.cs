namespace TaskManagementSystem.Shared.Models;

public record CalendarGetEventsRequest(DateTimeOffset StartTime, DateTimeOffset EndTime);

public record CalendarResponse(bool IsSuccess, CalendarEvent[]? Events, string? ErrorDescription);