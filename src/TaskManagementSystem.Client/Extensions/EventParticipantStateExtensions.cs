using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Extensions;

public static class EventParticipantStateExtensions
{
	public static string GetIcon(this EventParticipantState? state)
	{
		return state switch
		{
			EventParticipantState.Confirmed => "oi-check text-success",
			EventParticipantState.Rejected => "oi-x text-danger",
			EventParticipantState.Unknown => "oi-question-mark text-info fw-bold",
			null => string.Empty,
			_ => throw new ArgumentOutOfRangeException(nameof(state))
		};
	}
	
	public static string GetDescription(this EventParticipantState state)
	{
		return state switch
		{
			EventParticipantState.Confirmed => "Подтверждено",
			EventParticipantState.Rejected => "Отклонено",
			EventParticipantState.Unknown => "Под вопросом",
			_ => throw new ArgumentOutOfRangeException(nameof(state))
		};
	}
}