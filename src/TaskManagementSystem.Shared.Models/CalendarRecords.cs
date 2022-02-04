namespace TaskManagementSystem.Shared.Models;

public record CalendarGetEventsRequest(DateTimeOffset StartTime, DateTimeOffset EndTime);

public record CalendarResponse(bool IsSuccess, CalendarEventInfo[]? Events, string? ErrorDescription);