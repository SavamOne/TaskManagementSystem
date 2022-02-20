using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Models;

public record CalendarWithParticipants
{
    public CalendarWithParticipants(Calendar calendar, ISet<CalendarParticipant> participants)
    {
        Calendar = calendar.AssertNotNull();
        Participants = participants.AssertNotNull();
    }

    public Calendar Calendar { get; }
    
    public ISet<CalendarParticipant> Participants { get; }
}