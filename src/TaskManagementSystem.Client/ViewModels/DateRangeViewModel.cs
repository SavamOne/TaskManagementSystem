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

		var eventResult = await serverProxy.GetEventsInPeriod(new GetCalendarEventsInPeriodRequest(calendarId, FirstDay.DateTimeOffset, LastDay.DateTimeOffset.AddDays(1)));

		Console.WriteLine(JsonSerializer.Serialize(eventResult, ApplicationJsonOptions.Options));

		if (!eventResult.IsSuccess)
		{
			toastService.AddSystemErrorToast(eventResult.ErrorDescription!);
		}

		var unknownNames = EventInfoViewModel.GetUnknownCalendarNames(eventResult.Value!);
		if (unknownNames.Any())
		{
			var calendarNamesResult = await serverProxy.GetCalendarName(new GetCalendarNameRequest(unknownNames));

			if (!calendarNamesResult.IsSuccess)
			{
				toastService.AddSystemErrorToast(calendarNamesResult.ErrorDescription!);
			}
			
			EventInfoViewModel.AddCalendarName(calendarNamesResult.Value!);
		}

		var events = eventResult.Value!
		   .Select(x => new EventInfoViewModel(x))
		   .ToList();

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