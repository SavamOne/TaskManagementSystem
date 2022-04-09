using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Dal.Extensions;

namespace TaskManagementSystem.BusinessLogic.Dal.Converters;

public static class CalendarEventConverter
{
	public static DalEvent ToDalEvent(this CalendarEvent calendarEvent)
	{
		return new DalEvent
		{
			Id = calendarEvent.Id,
			Descrption = calendarEvent.Description,
			Name = calendarEvent.Name,
			Place = calendarEvent.Place,
			CalendarId = calendarEvent.CalendarId,
			CreationTime = calendarEvent.CreationTimeUtc,
			EndTime = calendarEvent.EndTimeUtc,
			EventType = (int)calendarEvent.EventType,
			IsPrivate = calendarEvent.IsPrivate,
			StartTime = calendarEvent.StartTimeUtc,
			IsRepeated = calendarEvent.IsRepeated
		};
	}

	public static CalendarEvent ToCalendarEvent(this DalEvent dalEvent)
	{
		return new CalendarEvent(dalEvent.Id,
			dalEvent.CalendarId,
			dalEvent.Name,
			dalEvent.Descrption,
			(EventType)dalEvent.EventType,
			dalEvent.Place,
			dalEvent.StartTime.SetUtcKind(),
			dalEvent.EndTime.SetUtcKind(),
			dalEvent.IsPrivate,
			dalEvent.CreationTime.SetUtcKind(),
			dalEvent.IsRepeated);
	}
}