using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public class CalendarParticipantUser
{
    public CalendarParticipantUser(
        Guid id,
        Guid calendarId,
        Guid userId,
        DateTimeOffset calendarJoinDate,
        CalendarParticipantRole role,
        string username,
        string email,
        DateTimeOffset registerDate)
    {
        Username = username.AssertNotNullOrWhiteSpace();
        Email = email.AssertNotNullOrWhiteSpace();

        Id = id;
        CalendarId = calendarId;
        UserId = userId;
        CalendarJoinDate = calendarJoinDate;
        Role = role;
        RegisterDate = registerDate;
    }

    public Guid Id { get; }

    public Guid CalendarId { get; }

    public Guid UserId { get; }

    public DateTimeOffset CalendarJoinDate { get; }

    public CalendarParticipantRole Role { get; }

    public string Username { get; }

    public string Email { get; }

    public DateTimeOffset RegisterDate { get; }
}