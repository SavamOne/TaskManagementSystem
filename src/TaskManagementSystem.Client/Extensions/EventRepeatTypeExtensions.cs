using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Extensions;

public static class EventRepeatTypeExtensions
{
	public static string GetDescription(EventRepeatType repeatType)
	{
		return repeatType switch
		{
			EventRepeatType.None => "Без повтора",
			EventRepeatType.EveryDay => "Ежедневно",
			EventRepeatType.EveryMonth => "Ежемесячно",
			EventRepeatType.EveryWeek => "Еженедельно",
			EventRepeatType.EveryYear => "Ежегодно",
			EventRepeatType.OnWeekDays => "По дням недели",
			_ => throw new ArgumentOutOfRangeException(nameof(repeatType), repeatType, null)
		};
	}
}