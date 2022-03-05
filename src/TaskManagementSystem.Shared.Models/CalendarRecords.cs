namespace TaskManagementSystem.Shared.Models;

public record CreateCalendarRequest(string Name, string Description);

public record EditCalendarRequest(Guid CalendarId, string? Name, string? Description);

public record AddCalendarParticipantRequest(Guid UserId, CalendarParticipantRole Role);

public record AddCalendarParticipantsRequest(Guid CalendarId, IEnumerable<AddCalendarParticipantRequest> Users);

public record ChangeCalendarParticipantRoleRequest(Guid ParticipantId, CalendarParticipantRole Role);

public record ChangeCalendarParticipantsRoleRequest(Guid CalendarId, IEnumerable<ChangeCalendarParticipantRoleRequest> Participants);

public record DeleteParticipantsRequest(Guid CalendarId, IEnumerable<Guid> ParticipantIds);

public record GetCalendarInfoRequest(Guid CalendarId);





