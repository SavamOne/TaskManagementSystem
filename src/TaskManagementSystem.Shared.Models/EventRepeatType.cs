namespace TaskManagementSystem.Shared.Models;

/// <summary>
/// Период повторения событий.
/// </summary>
public enum EventRepeatType
{
	/// <summary>
	/// Не повторять.
	/// </summary>
	None = 0,
	
	/// <summary>
	/// Каждый день.
	/// </summary>
	EveryDay = 1,
	
	/// <summary>
	/// Каждую неделю.
	/// </summary>
	EveryWeek = 2,
	
	/// <summary>
	/// Каждый месяц.
	/// </summary>
	EveryMonth = 3,
	
	/// <summary>
	/// Каждый год.
	/// </summary>
	EveryYear = 4,
	
	/// <summary>
	/// По дням недели.
	/// </summary>
	OnWeekDays = 5
}