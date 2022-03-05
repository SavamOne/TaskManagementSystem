using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Models;

public record CalendarWithParticipantsAndUsers
{
    public CalendarWithParticipantsAndUsers(Calendar calendar, ISet<CalendarParticipant> participants, ISet<User> users)
    {
        Calendar = calendar.AssertNotNull();
        Participants = participants.AssertNotNull();
        Users = users.AssertNotNull();
    }

    public Calendar Calendar { get; }
    
    public ISet<CalendarParticipant> Participants { get; }
    
    public ISet<User> Users { get; }
}