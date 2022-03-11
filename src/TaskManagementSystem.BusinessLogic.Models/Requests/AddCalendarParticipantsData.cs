using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class AddCalendarParticipantsData
{
    public AddCalendarParticipantsData(Guid adderId, Guid calendarId, ISet<AddCalendarParticipantData> users)
    {
        AdderId = adderId;
        CalendarId = calendarId;
        Users = users.AssertNotNull();
    }

    public Guid AdderId { get; }

    public Guid CalendarId { get; }

    public ISet<AddCalendarParticipantData> Users { get; }
}