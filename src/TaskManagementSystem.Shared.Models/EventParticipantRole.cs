namespace TaskManagementSystem.Shared.Models;

/// <summary>
///     Роль участника календаря.
/// </summary>
public enum EventParticipantRole
{
	/// <summary>
	///     Не выставлена (по умолчанию).
	/// </summary>
	NotSet = -1,

	/// <summary>
	///     Информируемый.
	/// </summary>
	Inform = 0,

	/// <summary>
	///     Участник.
	/// </summary>
	Participant = 1,

	/// <summary>
	///     Создатель.
	/// </summary>
	Creator = 2
}