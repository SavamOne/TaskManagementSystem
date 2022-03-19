namespace TaskManagementSystem.Shared.Models;

public record CalendarGetEventsRequest(DateTimeOffset StartTime, DateTimeOffset EndTime);


public record CreateEventRequest(Guid CalendarId, string Name, string? Description, string? Place, CalendarEventType Type, DateTimeOffset StartTime, DateTimeOffset? EndTime, bool IsPrivate);

public record EditEventRequest(Guid EventId, string? Name, string? Description, string? Place, CalendarEventType Type, DateTimeOffset? StartTime, DateTimeOffset? EndTime, bool? IsPrivate);

public record DeleteEventRequest(Guid EventId);

public record AddEventParticipantRequest(Guid ParticipantId, EventParticipantRole Role);

public record AddEventParticipantsRequest(Guid EventId, ICollection<AddEventParticipantRequest> Participants);

public record GetEventInfoRequest(Guid EventId);

public record GetEventsInPeriodRequest(Guid CalendarId, DateTimeOffset StartPeriod, DateTimeOffset EndPeriod);
