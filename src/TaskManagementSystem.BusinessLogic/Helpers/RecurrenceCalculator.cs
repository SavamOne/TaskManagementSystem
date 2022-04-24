using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Helpers;

public static class RecurrenceCalculator
{
	public static IEnumerable<CalendarEvent> Calculate(CalendarEvent @event,
		RecurrentEventSettings settings,
		DateTime calendarStartTime,
		DateTime calendarEndTime)
	{
		if (settings.RepeatType == RepeatType.OnWeekDays)
		{
			return ByDaysOfWeekCalculator.Calculate(@event, settings, calendarStartTime, calendarEndTime);
		}

		return ByPeriodRecurrenceCalculator.Calculate(@event, settings, calendarStartTime, calendarEndTime);
	}
}