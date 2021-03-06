using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.Converters;

public static class CalendarEventParticipantConverter
{
	public static DalEventParticipant ToDalEventParticipant(this CalendarEventParticipant participant)
	{
		return new DalEventParticipant
		{
			Id = participant.Id,
			EventId = participant.EventId,
			CalendarParticipantId = participant.CalendarParticipantId,
			Role = (int)participant.Role,
			StartNotificationBefore = participant.NotifyBefore,
			StatusType = (int)participant.State
		};
	}

	public static CalendarEventParticipant ToEventParticipant(this DalEventParticipant dalEventParticipant)
	{
		return new CalendarEventParticipant(dalEventParticipant.Id,
			dalEventParticipant.EventId,
			dalEventParticipant.CalendarParticipantId,
			(CalendarEventParticipantRole)dalEventParticipant.Role,
			(CalendarEventParticipantState)dalEventParticipant.StatusType)
		{
			NotifyBefore = dalEventParticipant.StartNotificationBefore,
			CalendarParticipant = dalEventParticipant.CalendarParticipant?.ToCalendarParticipant()
		};
	}
}