namespace TaskManagementSystem.Shared.Models;

public record CreateEventRequest(Guid CalendarId, string Name, string? Description, string? Place, CalendarEventType Type, DateTimeOffset StartTime, DateTimeOffset? EndTime, bool IsPrivate);

public record EditEventRequest(Guid EventId, string? Name, string? Description, string? Place, CalendarEventType Type, DateTimeOffset? StartTime, DateTimeOffset? EndTime, bool? IsPrivate);

public record DeleteEventRequest(Guid EventId);

public record AddEventParticipantRequest(Guid ParticipantId, EventParticipantRole Role);

public record AddEventParticipantsRequest(Guid EventId, ICollection<AddEventParticipantRequest> Participants);

public record ChangeEventParticipantRequest(Guid EventParticipantId, EventParticipantRole? Role, bool Delete);

public record ChangeEventParticipantsRequest(Guid EventId, ICollection<ChangeEventParticipantRequest> Participants);

public record GetEventInfoRequest(Guid EventId);

public record GetEventsInPeriodRequest(Guid CalendarId, DateTimeOffset StartPeriod, DateTimeOffset EndPeriod);
