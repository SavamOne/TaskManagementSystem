using System.Globalization;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Client.ViewModels;

public class DayViewModel
{
    private static readonly DateTimeFormatInfo DateTimeFormat = CultureInfo.CurrentUICulture.DateTimeFormat.Clone() as DateTimeFormatInfo;
    
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

    public IEnumerable<EventViewModel> Events { get; private set; } = Enumerable.Empty<EventViewModel>();

    public void InterceptDateEvents(IEnumerable<EventViewModel> allEvents)
    {
        Events = allEvents.Where(x => DateRangeHelper.IsEventInCalendarDay(x.StartDate, x.EndDate, Date)).ToList();
    }

    public void ClearEvents()
    {
        Events = Enumerable.Empty<EventViewModel>();
    }
}