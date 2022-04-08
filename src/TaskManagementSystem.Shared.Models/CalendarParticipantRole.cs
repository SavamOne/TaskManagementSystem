namespace TaskManagementSystem.Shared.Models;

/// <summary>
/// Роль участника календаря.
/// </summary>
public enum CalendarParticipantRole
{
	/// <summary>
	/// Не выставлена (по умолчанию).
	/// </summary>
	NotSet = -1,
	
	/// <summary>
	/// Участник.
	/// </summary>
	Participant = 0,
	
	/// <summary>
	/// Администратор.
	/// </summary>
	Admin = 1,
	
	/// <summary>
	/// Cоздатель.
	/// </summary>
	Creator = 2
}