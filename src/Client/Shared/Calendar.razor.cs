using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace TaskManagementSystem.Client.Shared;

public partial class Calendar
{
    private readonly DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentUICulture.DateTimeFormat.Clone() as DateTimeFormatInfo;
    
    [Parameter]
    public DateOnly WorkingDate { get; set; } = GetDefaultWorkingDate();

    [Parameter]
    public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Monday;

    protected override Task OnInitializedAsync()
    {
        UpdateCurrentMonthState();
        UpdateFirstDayOfWeekState();
        UpdatePreviousAndNextMonthsState();
        
        return base.OnInitializedAsync();
    }

    private bool IsLoaded => PreviousMonthDaysEnumerable != null && MonthDaysEnumerable != null && NextMonthDaysEnumerable != null;

    private static IEnumerable<DayOfWeek> DayOfWeekValues { get; } = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();
    
    private string Month { get; set; }

    private int Year { get; set; }
    
    private int MonthStartingDayOfWeek { get; set; }

    private IEnumerable<int> MonthDaysEnumerable { get; set; }

    private IEnumerable<int> PreviousMonthDaysEnumerable { get; set; }

    private IEnumerable<int> NextMonthDaysEnumerable { get; set; }

    private IEnumerable<string> DayOfWeekNamesWithFirstDay { get; set; }
    
    private string GetDayName(DayOfWeek dayOfWeek) => dateTimeFormat.GetShortestDayName(dayOfWeek);
    
    private void AppendMonth()
    {
        WorkingDate = WorkingDate.AddMonths(1);
        UpdateCurrentMonthState();
        UpdatePreviousAndNextMonthsState();
    }

    private void RemoveMonth()
    {
        WorkingDate = WorkingDate.AddMonths(-1);
        UpdateCurrentMonthState();
        UpdatePreviousAndNextMonthsState();
    }

    private void ChangeFirstDayOfWeek(DayOfWeek day)
    {
        FirstDayOfWeek = day;
        UpdateFirstDayOfWeekState();
        UpdatePreviousAndNextMonthsState();
    }

    private int GetDayOfWeek(DateOnly date)
    {
        int day = (int)date.DayOfWeek;
        int calendarFirstDay = (int)FirstDayOfWeek;
        int mod = (day - calendarFirstDay) % 7;

        return mod >= 0 ? mod : mod + 7;
    }

    private void UpdateCurrentMonthState()
    {
        Month = dateTimeFormat.GetMonthName(WorkingDate.Month);
        Year = WorkingDate.Year;
        int daysInMonth = DateTime.DaysInMonth(WorkingDate.Year, WorkingDate.Month);
        MonthDaysEnumerable = Enumerable.Range(1, daysInMonth).ToList();
    }

    private void UpdatePreviousAndNextMonthsState()
    {
        MonthStartingDayOfWeek = GetDayOfWeek(WorkingDate);
        
        DateOnly previousMonth = WorkingDate.AddMonths(-1);
        int daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
        int daysCount = MonthStartingDayOfWeek - 1;

        PreviousMonthDaysEnumerable = Enumerable.Range(daysInPreviousMonth - daysCount, daysCount + 1).ToList();
        
        DateOnly nextMonth = WorkingDate.AddMonths(1);
        int nextMonthFirstDayOfWeek = GetDayOfWeek(nextMonth);
        daysCount = (7 - nextMonthFirstDayOfWeek) % 7;
        NextMonthDaysEnumerable = Enumerable.Range(1, daysCount).ToList();
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