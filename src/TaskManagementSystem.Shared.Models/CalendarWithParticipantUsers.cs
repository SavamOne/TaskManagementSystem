using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public class CalendarWithParticipantUsers
{
    public CalendarWithParticipantUsers(CalendarInfo calendar, IEnumerable<CalendarParticipantUser> participants)
    {
        Calendar = calendar.AssertNotNull();
        Participants = participants.AssertNotNull();
    }

    public CalendarInfo Calendar { get; }

    public IEnumerable<CalendarParticipantUser> Participants { get; }
}