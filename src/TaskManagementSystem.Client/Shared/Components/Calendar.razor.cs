using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Shared.Components.Modals;
using TaskManagementSystem.Client.ViewModels;
using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Options;

namespace TaskManagementSystem.Client.Shared.Components;

public partial class Calendar
{
    private Modal Modal { get; set; } = new();

    private EventViewModel SelectedEvent { get; set; } = new();
    
    private readonly DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentUICulture.DateTimeFormat.Clone() as DateTimeFormatInfo;

    [Parameter]
    public DateOnly WorkingDate { get; set; } = GetDefaultWorkingDate();

    [Parameter]
    public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Sunday;
    
    [Inject]
    public ServerProxy ServerProxy { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoaded = false;
        
        UpdateCurrentMonthState();
        UpdateFirstDayOfWeekState();
        UpdatePreviousAndNextMonthsState();
        await GetEventsAsync();

        IsLoaded = true;
    }

    private bool IsLoaded { get; set; }

    private static IEnumerable<DayOfWeek> DayOfWeekValues { get; } = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();
    
    private string Month { get; set; }

    private int Year { get; set; }
    
    private int MonthStartingDayOfWeek { get; set; }

    private IEnumerable<DayViewModel> MonthDaysEnumerable { get; set; }

    private IEnumerable<DayViewModel> PreviousMonthDaysEnumerable { get; set; }

    private IEnumerable<DayViewModel> NextMonthDaysEnumerable { get; set; }

    private IEnumerable<string> DayOfWeekNamesWithFirstDay { get; set; }
    
    private string GetDayName(DayOfWeek dayOfWeek) => dateTimeFormat.GetShortestDayName(dayOfWeek);
    
    private async Task AppendMonth()
    {
        IsLoaded = false;
        
        WorkingDate = WorkingDate.AddMonths(1);
        UpdateCurrentMonthState();
        UpdatePreviousAndNextMonthsState();
        await GetEventsAsync();

        IsLoaded = true;
    }

    private async Task RemoveMonth()
    {
        IsLoaded = false;
        
        WorkingDate = WorkingDate.AddMonths(-1);
        UpdateCurrentMonthState();
        UpdatePreviousAndNextMonthsState();
        await GetEventsAsync();
        
        IsLoaded = true;
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
        
        MonthDaysEnumerable = Enumerable.Range(1, daysInMonth).Select(day => new DayViewModel(new DateOnly(Year, WorkingDate.Month, day), false)).ToList();
    }

    private void UpdatePreviousAndNextMonthsState()
    {
        MonthStartingDayOfWeek = GetDayOfWeek(WorkingDate);
        
        DateOnly previousMonth = WorkingDate.AddMonths(-1);
        int daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
        int daysCount = MonthStartingDayOfWeek - 1;

        PreviousMonthDaysEnumerable = Enumerable.Range(daysInPreviousMonth - daysCount, daysCount + 1)
            .Select(day => new DayViewModel(new DateOnly(previousMonth.Year, previousMonth.Month, day), true))
            .ToList();
        
        DateOnly nextMonth = WorkingDate.AddMonths(1);
        int nextMonthFirstDayOfWeek = GetDayOfWeek(nextMonth);
        daysCount = (7 - nextMonthFirstDayOfWeek) % 7;
        NextMonthDaysEnumerable = Enumerable.Range(1, daysCount)
            .Select(day => new DayViewModel(new DateOnly(nextMonth.Year, nextMonth.Month, day), true))
            .ToList();
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


    private async Task GetEventsAsync()
    {
        DayViewModel firstDate = PreviousMonthDaysEnumerable.FirstOrDefault() ?? MonthDaysEnumerable.First();
        DayViewModel lastDate = NextMonthDaysEnumerable.LastOrDefault() ?? MonthDaysEnumerable.Last();
        
        CalendarResponse result = await ServerProxy.GetEventsForMonth(new CalendarGetEventsRequest(firstDate.DateTimeOffset, lastDate.DateTimeOffset.AddDays(1)));
        Console.WriteLine(JsonSerializer.Serialize(result, ApplicationJsonOptions.Options));

        if (!result.IsSuccess)
        {
            throw new Exception(result.ErrorDescription);
        }

        var events = result.Events.Select(x => new EventViewModel
        {
            Name = x.Name,
            StartDate = x.StartTime,
            EndDate = x.EndTime
        }).ToList();

        foreach (DayViewModel dayViewModel in PreviousMonthDaysEnumerable.Union(MonthDaysEnumerable).Union(NextMonthDaysEnumerable))
        {
            dayViewModel.InterceptDateEvents(events);
            events.RemoveAll(x => x.EndDate < dayViewModel.DateTimeOffset.AddDays(1));
        }
    }
    
    private void EventClicked(EventViewModel ev)
    {
        SelectedEvent = ev;
        Modal.Open();
    }
}