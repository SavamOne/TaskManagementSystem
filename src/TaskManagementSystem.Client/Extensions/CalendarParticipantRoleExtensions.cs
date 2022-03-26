using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Extensions;

public static class CalendarParticipantRoleExtensions
{
	public static string GetDescription(this CalendarParticipantRole role)
	{
		return role switch
		{
			CalendarParticipantRole.Admin => "Администратор",
			CalendarParticipantRole.Creator => "Создатель календаря",
			CalendarParticipantRole.Participant => "Участник",
			CalendarParticipantRole.NotSet => "Не участник",
			_ => throw new ArgumentOutOfRangeException(nameof(role))
		};
	}
}