using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Models;

public record CalendarWithParticipants
{
    public CalendarWithParticipants(Calendar calendar, ICollection<CalendarParticipant> participants)
    {
        Calendar = calendar.AssertNotNull();
        Participants = participants.AssertNotNull();
    }

    public Calendar Calendar { get; }

    public ICollection<CalendarParticipant> Participants { get; }
}