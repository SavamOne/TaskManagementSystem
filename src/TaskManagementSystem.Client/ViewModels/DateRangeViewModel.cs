using System.Text.Json;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Shared.Models.Options;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.ViewModels;

public class DateRangeViewModel
{
	private readonly Guid calendarId;
	private readonly ServerProxy serverProxy;
	private readonly IToastService toastService;

	public DateRangeViewModel(ServerProxy serverProxy,
		IToastService toastService,
		IEnumerable<DayViewModel> days,
		Guid calendarId)
	{
		this.serverProxy = serverProxy;
		this.toastService = toastService;
		this.calendarId = calendarId;
		Days = days;
	}

	public IEnumerable<DayViewModel> Days { get; }

	public DayViewModel FirstDay => Days.First();

	public DayViewModel LastDay => Days.Last();

	public async Task GetEventsAsync()
	{
		ClearEvents();

		var result = await serverProxy.GetEventsInPeriod(new GetEventsInPeriodRequest(calendarId, FirstDay.DateTimeOffset, LastDay.DateTimeOffset.AddDays(1)));

		Console.WriteLine(JsonSerializer.Serialize(result, ApplicationJsonOptions.Options));

		if (!result.IsSuccess)
		{
			toastService.AddSystemErrorToast(result.ErrorDescription!);
		}

		var events = result.Value!.ToList();

		foreach (DayViewModel dayViewModel in Days)
		{
			dayViewModel.InterceptDateEvents(events);
			events.RemoveAll(x => x.EndTime < dayViewModel.DateTimeOffset.AddDays(1));
		}
	}

	private void ClearEvents()
	{
		foreach (DayViewModel dayViewModel in Days)
		{
			dayViewModel.ClearEvents();
		}
	}
}