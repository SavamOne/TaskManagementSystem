using System.Globalization;
using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Components.Modals;
using TaskManagementSystem.Client.Helpers.Implementations;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Client.ViewModels;

namespace TaskManagementSystem.Client.Components;

public abstract partial class CalendarEventComponent
{
	[Parameter]
	public DateOnly WorkingDate { get; set; } = GetDefaultWorkingDate();

	[Parameter]
	public Guid CalendarId { get; set; }

	[Inject]
	public ServerProxy? ServerProxy { get; set; }

	[Inject]
	public IToastService? ToastService { get; set; }

	[Inject]
	public ILocalizationService? LocalizationService { get; set; }

	private EventEditFormModal EditEventModal { get; set; } = new();
	
	private CalendarInfoModal CalendarInfoModal{ get; set; } = new();

	private CultureInfo? CultureInfo { get; set; }

	private ICollection<DayViewModel> Days { get; set; } = Array.Empty<DayViewModel>();

	private bool IsLoaded { get; set; }

	private string? Month { get; set; }

	private int Year { get; set; }

	private ICollection<DayOfWeekViewModel>? DayOfWeekNamesWithFirstDay { get; set; }
	
	protected string? CalendarName { get; set; }

	protected override async Task OnInitializedAsync()
	{
		IsLoaded = false;

		await OnLoadAsync();
		
		IsLoaded = true;
	}

	protected virtual async Task OnLoadAsync()
	{
		CultureInfo = await LocalizationService!.GetApplicationCultureAsync();
		DayOfWeekNamesWithFirstDay = DayOfWeekHelper.GetDayOfWeeksOrderedByFirstDay(CultureInfo!, true);

		await UpdateCurrentMonthStateAsync();
	}
	
	protected abstract Task<IEnumerable<EventInfoViewModel>> GetEventsInDateRange(DateTimeOffset startTime, DateTimeOffset endTime);

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

	private int GetDayOfWeek(DateOnly date)
	{
		int day = (int)date.DayOfWeek;
		int calendarFirstDay = (int)CultureInfo!.DateTimeFormat.FirstDayOfWeek;
		int mod = ( day - calendarFirstDay ) % 7;

		return mod >= 0 ? mod : mod + 7;
	}

	private async Task UpdateCurrentMonthStateAsync()
	{
		Month = CultureInfo!.DateTimeFormat.GetMonthName(WorkingDate.Month);
		Year = WorkingDate.Year;

		int daysInMonth = DateTime.DaysInMonth(WorkingDate.Year, WorkingDate.Month);
		var monthDaysEnumerable = Enumerable.Range(1, daysInMonth)
		   .Select(day => new DayViewModel(new DateOnly(WorkingDate.Year, WorkingDate.Month, day), false))
		   .ToList();

		DateOnly previousMonth = WorkingDate.AddMonths(-1);
		int daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
		int previousDaysCount = GetDayOfWeek(WorkingDate) - 1;

		var previousMonthDaysEnumerable = Enumerable.Range(daysInPreviousMonth - previousDaysCount, previousDaysCount + 1)
		   .Select(day => new DayViewModel(new DateOnly(previousMonth.Year, previousMonth.Month, day), true))
		   .ToList();

		DateOnly nextMonth = WorkingDate.AddMonths(1);
		int nextMonthFirstDayOfWeek = GetDayOfWeek(nextMonth);
		int nextDaysCount = ( 7 - nextMonthFirstDayOfWeek ) % 7;
		var nextMonthDaysEnumerable = Enumerable.Range(1, nextDaysCount)
		   .Select(day => new DayViewModel(new DateOnly(nextMonth.Year, nextMonth.Month, day), true))
		   .ToList();

		Days = previousMonthDaysEnumerable.Union(monthDaysEnumerable).Union(nextMonthDaysEnumerable).ToList(); 
		
		await FillDays();
	}

	private async Task FillDays()
	{
		DateTimeOffset firstDay = Days.FirstOrDefault()?.DateTimeOffset ?? DateTimeOffset.Now;
		DateTimeOffset lastDay = Days.LastOrDefault()?.DateTimeOffset.AddDays(1) ?? DateTimeOffset.Now;
		var events = (await GetEventsInDateRange(firstDay, lastDay)).ToList();
		
		foreach (DayViewModel dayViewModel in Days)
		{
			dayViewModel.InterceptDateEvents(events);
			events.RemoveAll(x => x.EndTime < dayViewModel.DateTimeOffset.AddDays(1));
		}
	}

	private static DateOnly GetDefaultWorkingDate()
	{
		DateTime current = DateTime.UtcNow;
		DateTime currentMonth = new(current.Year, current.Month, 1);

		return DateOnly.FromDateTime(currentMonth);
	}
}