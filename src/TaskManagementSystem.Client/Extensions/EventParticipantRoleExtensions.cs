using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Extensions;

public static class EventParticipantRoleExtensions
{
	public static string GetDescription(this EventParticipantRole role)
	{
		return role switch
		{
			EventParticipantRole.Creator => "Создатель события",
			EventParticipantRole.Participant => "Участник",
			EventParticipantRole.Inform => "Информируемый",
			EventParticipantRole.NotSet => "Не участник",
			_ => throw new ArgumentOutOfRangeException(nameof(role))
		};
	}
}