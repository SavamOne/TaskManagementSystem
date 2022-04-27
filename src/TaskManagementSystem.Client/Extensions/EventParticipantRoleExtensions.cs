using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Extensions;

public static class EventParticipantRoleExtensions
{
	public static string GetDescription(this EventParticipantRole role, CalendarEventType eventType)
	{
		return role switch
		{
			EventParticipantRole.Creator => "Создатель",
			EventParticipantRole.Participant => eventType switch
			{

				CalendarEventType.Unknown => "Участник",
				CalendarEventType.Event => "Участник",
				CalendarEventType.Meeting => "Участник",
				CalendarEventType.Call => "Собеседник",
				CalendarEventType.Task => "Исполнитель",
				CalendarEventType.Reminder => "Участник",
				_ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null)
			},
			EventParticipantRole.Inform => "Информируемый",
			EventParticipantRole.NotSet => "Роль не выставлена",
			_ => throw new ArgumentOutOfRangeException(nameof(role))
		};
	}
}