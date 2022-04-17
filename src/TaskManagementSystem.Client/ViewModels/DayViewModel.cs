using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.ViewModels;

public class DayViewModel
{
	public DayViewModel(DateOnly date, bool isHidden)
	{
		Date = date;
		IsHidden = isHidden;
	}

	public DateOnly Date { get; }

	public DateTimeOffset DateTimeOffset => Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local);

	public int Day => Date.Day;

	public DayOfWeek DayOfWeek => Date.DayOfWeek;

	public bool IsHidden { get; }

	public bool IsWeekEndDay => DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

	public bool IsTodayDay => Date == DateOnly.FromDateTime(DateTime.Today);

	public ICollection<EventInfoViewModel> Events { get; private set; } = Array.Empty<EventInfoViewModel>();

	public void InterceptDateEvents(IEnumerable<EventInfoViewModel> allEvents)
	{
		Events = allEvents.Where(x => DateRangeHelper.IsEventInCalendarDay(x.StartTime, x.EndTime, Date))
		   .ToList();
	}

	public void ClearEvents()
	{
		Events.ClearIfPossible();
	}
}