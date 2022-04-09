using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Helpers;

public static class ByPeriodRecurrenceCalculator
{
	private static Dictionary<RepeatType, Func<DateTime, DateTime>> ConvertFuncDict = new()
	{
		{
			RepeatType.EveryDay, time => time.AddDays(1)
		},
		{
			RepeatType.EveryWeek, time => time.AddDays(7)
		},
		{
			RepeatType.EveryMonth, time => time.AddMonths(1)
		},
		{
			RepeatType.EveryYear, time => time.AddYears(1)
		}
	};
	
	public static IEnumerable<CalendarEvent> Calculate(CalendarEvent @event, RecurrentEventSettings settings, DateTime calendarStartTime, DateTime calendarEndTime)
	{
		var periodFunc = ConvertFuncDict[settings.RepeatType];
		uint maxEventCount = settings.RepeatCount ?? uint.MaxValue;
		DateTime until = settings.UntilUtc ?? DateTime.MaxValue;

		DateTime eventStartTime = @event.StartTimeUtc;
		DateTime eventEndTime = @event.EndTimeUtc;
		uint eventCount = 1;

		if (calendarStartTime > until)
		{
			yield break;
		}

		while (eventStartTime <= calendarEndTime && eventCount <= maxEventCount && eventStartTime <= until)
		{
			if (DateRangeHelper.IsEventInCalendarRange(eventStartTime, eventEndTime, calendarStartTime, calendarEndTime))
			{
				yield return @event with
				{
					StartTimeUtc = eventStartTime,
					EndTimeUtc = eventEndTime,
					RepeatNum = eventCount,
				};
			}

			eventStartTime = periodFunc(eventStartTime);
			eventEndTime = periodFunc(eventEndTime);
			eventCount++;
		}
	}
}