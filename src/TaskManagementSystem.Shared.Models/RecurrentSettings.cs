using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Shared.Models;

/// <summary>
///     Настройки повторения события.
/// </summary>
public class RecurrentSettings
{
	public RecurrentSettings(EventRepeatType repeatType,
		ISet<DayOfWeek>? dayOfWeeks,
		uint? count,
		DateTimeOffset? until)
	{
		Count = count;
		Until = until;
		RepeatType = repeatType;
		DayOfWeeks = dayOfWeeks;
	}

	/// <summary>
	///     Ограничение повторения по количеству. null - отсутствие ограничения.
	/// </summary>
	public uint? Count { get; }

	/// <summary>
	///     Ограничение повторения до определенной даты. null - отсутствие ограничения.
	/// </summary>
	public DateTimeOffset? Until { get; }

	/// <summary>
	///     Вид повторения.
	/// </summary>
	[Required]
	public EventRepeatType RepeatType { get; }

	/// <summary>
	///     Дни недели. Учитывается только если <see cref="RepeatType" />==<see cref="EventRepeatType.OnWeekDays" />.
	/// </summary>
	public ISet<DayOfWeek>? DayOfWeeks { get; }
}