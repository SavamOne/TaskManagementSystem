namespace TaskManagementSystem.Shared.Models;

/// <summary>
///     Тип события.
/// </summary>
public enum CalendarEventType
{
	/// <summary>
	///     Неизвестный.
	/// </summary>
	Unknown = 0,

	/// <summary>
	///     Событие.
	/// </summary>
	Event,

	/// <summary>
	///     Встреча.
	/// </summary>
	Meeting,

	/// <summary>
	///     Звонок.
	/// </summary>
	Call,

	/// <summary>
	///     Задача.
	/// </summary>
	Task,

	/// <summary>
	///     Напоминание.
	/// </summary>
	Reminder
}