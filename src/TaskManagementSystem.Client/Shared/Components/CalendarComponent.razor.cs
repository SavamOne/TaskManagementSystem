using System.Globalization;
using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Shared.Components.Modals;
using TaskManagementSystem.Client.ViewModels;

namespace TaskManagementSystem.Client.Shared.Components;

public partial class CalendarComponent
{
    private readonly DateTimeFormatInfo dateTimeFormat = (CultureInfo.CurrentUICulture.DateTimeFormat.Clone() as DateTimeFormatInfo)!;

    [Parameter]
    public DateOnly WorkingDate { get; set; } = GetDefaultWorkingDate();

    [Parameter]
    public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Sunday;

    [Parameter]
    public EventEditFormModal EditEventModal { get; set; } = new();
    
    [Inject]
    public ServerProxy? ServerProxy { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoaded = false;
        
        UpdateFirstDayOfWeekState();
        await UpdateCurrentMonthStateAsync();
        
        IsLoaded = true;
    }

    private DateRangeViewModel? DateRangeViewModel { get; set; }
    
    private bool IsLoaded { get; set; }

    private string? Month { get; set; }

    private int Year { get; set; }

    private IEnumerable<string>? DayOfWeekNamesWithFirstDay { get; set; }
    
    private string GetDayName(DayOfWeek dayOfWeek) => dateTimeFormat.GetShortestDayName(dayOfWeek);
    
    private async Task AppendMonth()
    {
        IsLoaded = false;
        
        WorkingDate = WorkingDate.AddMonths(1);
        await UpdateCurrentMonthStateAsync();

        IsLoaded = true;
    }

    private async Task RemoveMonth()
    {
        IsLoaded = false;
        
        WorkingDate = WorkingDate.AddMonths(-1);
        await UpdateCurrentMonthStateAsync();

        IsLoaded = true;
    }

    private void ChangeFirstDayOfWeek(DayOfWeek day)
    {
        FirstDayOfWeek = day;
        UpdateFirstDayOfWeekState();
    }

    private int GetDayOfWeek(DateOnly date)
    {
        int day = (int)date.DayOfWeek;
        int calendarFirstDay = (int)FirstDayOfWeek;
        int mod = (day - calendarFirstDay) % 7;

        return mod >= 0 ? mod : mod + 7;
    }

    private async Task UpdateCurrentMonthStateAsync()
    {
        Month = dateTimeFormat.GetMonthName(WorkingDate.Month);
        Year = WorkingDate.Year;
        
        int daysInMonth = DateTime.DaysInMonth(WorkingDate.Year, WorkingDate.Month);
        List<DayViewModel> monthDaysEnumerable = Enumerable.Range(1, daysInMonth)
            .Select(day => new DayViewModel(new DateOnly(WorkingDate.Year, WorkingDate.Month, day), false))
            .ToList();

        DateOnly previousMonth = WorkingDate.AddMonths(-1);
        int daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
        int previousDaysCount = GetDayOfWeek(WorkingDate) - 1;

        List<DayViewModel> previousMonthDaysEnumerable = Enumerable.Range(daysInPreviousMonth - previousDaysCount, previousDaysCount + 1)
            .Select(day => new DayViewModel(new DateOnly(previousMonth.Year, previousMonth.Month, day), true))
            .ToList();
        
        DateOnly nextMonth = WorkingDate.AddMonths(1);
        int nextMonthFirstDayOfWeek = GetDayOfWeek(nextMonth);
        int nextDaysCount = (7 - nextMonthFirstDayOfWeek) % 7;
        List<DayViewModel> nextMonthDaysEnumerable = Enumerable.Range(1, nextDaysCount)
            .Select(day => new DayViewModel(new DateOnly(nextMonth.Year, nextMonth.Month, day), true))
            .ToList();

        DateRangeViewModel = new DateRangeViewModel(ServerProxy!, previousMonthDaysEnumerable.Union(monthDaysEnumerable).Union(nextMonthDaysEnumerable).ToList());
        await DateRangeViewModel.GetEventsAsync();
    }

    private void UpdateFirstDayOfWeekState()
    {
        DayOfWeekNamesWithFirstDay = Enumerable.Range((int)FirstDayOfWeek, 7)
            .Select(x => x % 7)
            .Cast<DayOfWeek>()
            .Select(GetDayName).ToList();
    }
    
    private static DateOnly GetDefaultWorkingDate()
    {
        DateTime current = DateTime.UtcNow;
        DateTime currentMonth = new(current.Year, current.Month, 1);
        
        return DateOnly.FromDateTime(currentMonth); 
    }
}