using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Extensions;

public static class CalendarEventTypeExtensions
{
	public static string GetDescription(this CalendarEventType eventType)
	{
		return eventType switch
		{
			CalendarEventType.Unknown => "По умолчанию",
			CalendarEventType.Call => "Звонок",
			CalendarEventType.Event => "Событие",
			CalendarEventType.Meeting => "Встреча",
			CalendarEventType.Reminder => "Напоминание",
			CalendarEventType.Task => "Задача",
			_ => throw new ArgumentOutOfRangeException(nameof(eventType))
		};
	}
}