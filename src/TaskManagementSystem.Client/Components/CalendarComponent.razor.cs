using System.Globalization;
using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Components.Modals;
using TaskManagementSystem.Client.Helpers.Implementations;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Client.ViewModels;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.Components;

public partial class CalendarComponent
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

	private DayOfWeek FirstDayOfWeek { get; set; }

	private EventEditFormModal EditEventModal { get; set; } = new();

	private CultureInfo? CultureInfo { get; set; }

	private DateRangeViewModel? DateRangeViewModel { get; set; }

	private bool IsLoaded { get; set; }

	private string? Month { get; set; }

	private int Year { get; set; }

	private ICollection<DayOfWeekViewModel>? DayOfWeekNamesWithFirstDay { get; set; }
	
	private string? CalendarName { get; set; }

	protected override async Task OnInitializedAsync()
	{
		IsLoaded = false;

		var result = await ServerProxy!.GetCalendarName(new GetCalendarNameRequest(new HashSet<Guid>
		{
			CalendarId
		}));

		if (!result.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(result.ErrorDescription!);
			return;
		}

		CalendarName = result.Value!.FirstOrDefault()?.Name;

		CultureInfo = await LocalizationService!.GetApplicationCultureAsync();
		FirstDayOfWeek = CultureInfo.DateTimeFormat.FirstDayOfWeek;

		UpdateFirstDayOfWeekState();
		await UpdateCurrentMonthStateAsync();

		IsLoaded = true;
	}

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

		DateRangeViewModel = new DateRangeViewModel(ServerProxy!,
			ToastService!,
			previousMonthDaysEnumerable.Union(monthDaysEnumerable).Union(nextMonthDaysEnumerable).ToList(),
			CalendarId);

		await DateRangeViewModel.GetEventsAsync();
	}

	private void UpdateFirstDayOfWeekState()
	{
		DayOfWeekNamesWithFirstDay = DayOfWeekHelper.GetDayOfWeeksOrderedByFirstDay(CultureInfo!, true);
	}

	private static DateOnly GetDefaultWorkingDate()
	{
		DateTime current = DateTime.UtcNow;
		DateTime currentMonth = new(current.Year, current.Month, 1);

		return DateOnly.FromDateTime(currentMonth);
	}
}