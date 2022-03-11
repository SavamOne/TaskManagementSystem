namespace TaskManagementSystem.Shared.Models;

public record CalendarGetEventsRequest(DateTimeOffset StartTime, DateTimeOffset EndTime);