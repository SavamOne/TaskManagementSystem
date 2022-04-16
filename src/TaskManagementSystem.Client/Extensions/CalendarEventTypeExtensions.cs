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

	public static string GetIcon(this CalendarEventType eventType)
	{
		return eventType switch
		{
			CalendarEventType.Unknown => string.Empty,
			CalendarEventType.Call => "oi-phone",
			CalendarEventType.Event => "oi-calendar",
			CalendarEventType.Meeting => "oi-people",
			CalendarEventType.Reminder => "oi-pin",
			CalendarEventType.Task => "oi-task",
			_ => throw new ArgumentOutOfRangeException(nameof(eventType))
		};
	}
}