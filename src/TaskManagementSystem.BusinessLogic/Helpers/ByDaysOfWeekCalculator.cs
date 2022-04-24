using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Helpers;

public static class ByDaysOfWeekCalculator
{
	public static IEnumerable<CalendarEvent> Calculate(CalendarEvent @event,
		RecurrentEventSettings settings,
		DateTime calendarStartTime,
		DateTime calendarEndTime)
	{
		byte[] matrix = CalculateMatrix(settings.DayOfWeeks!);

		uint maxEventCount = settings.RepeatCount ?? uint.MaxValue;
		DateTime until = settings.UntilUtc ?? DateTime.MaxValue;
		uint eventCount = 1;

		DateTime eventStartTime = @event.StartTimeUtc;
		DateTime eventEndTime = @event.EndTimeUtc;

		while (eventStartTime <= calendarEndTime && eventCount <= maxEventCount && eventStartTime <= until)
		{
			if (DateRangeHelper.IsEventInCalendarRange(eventStartTime, eventEndTime, calendarStartTime, calendarEndTime))
			{
				yield return @event with
				{
					StartTimeUtc = eventStartTime,
					EndTimeUtc = eventEndTime,
					RepeatNum = eventCount
				};
			}

			byte daysToAdd = matrix[(int)eventStartTime.DayOfWeek];
			eventStartTime = eventStartTime.AddDays(daysToAdd);
			eventEndTime = eventEndTime.AddDays(daysToAdd);
			eventCount++;
		}
	}

	private static byte[] CalculateMatrix(ISet<DayOfWeek> days)
	{
		byte[] matrix = new byte[7];

		var intDays = days.Select(x => (int)x).OrderBy(x => x).ToList();
		intDays.Add(intDays.First() + 7);

		int previousPlus = 0;
		foreach (int dayOfWeek in intDays)
		{
			byte a = 1;
			for (int i = dayOfWeek - 1; i >= previousPlus; i--)
			{
				if (i < 7)
				{
					matrix[i] = a;
				}

				a++;
			}
			previousPlus = dayOfWeek;
		}

		return matrix;
	}
}