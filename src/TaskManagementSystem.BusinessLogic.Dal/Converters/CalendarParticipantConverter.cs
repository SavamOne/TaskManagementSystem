using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Exceptions;
using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.Converters;

public static class CalendarParticipantConverter
{
    public static DalCalendarParticipant ToDalCalendarParticipant(this CalendarParticipant calendarParticipant)
    {
        return new DalCalendarParticipant
        {
            Id = calendarParticipant.Id,
            CalendarId = calendarParticipant.CalendarId,
            JoinDate = calendarParticipant.JoinDateUtc,
            UserId = calendarParticipant.UserId,
            Role = (int)calendarParticipant.Role,
            IsDeleted = false
        };
    }

    public static CalendarParticipant ToCalendarParticipant(this DalCalendarParticipant calendarParticipant)
    {
        if (calendarParticipant.IsDeleted)
        {
            throw new BusinessLogicException("Этот участник удален");
        }

        return new CalendarParticipant(calendarParticipant.Id,
            calendarParticipant.CalendarId,
            calendarParticipant.UserId,
            calendarParticipant.JoinDate,
            (CalendarRole)calendarParticipant.Role)
        {
            User = calendarParticipant.User?.ToUser()
        };
    }
}