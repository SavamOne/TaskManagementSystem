using System.Text.Json;
using TaskManagementSystem.Client.ViewModels;
using TaskManagementSystem.Shared.Models.Options;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.Components;

public class CalendarEventComponentWithCalendarEventEvents : CalendarEventComponent
{
	protected override async Task OnLoadAsync()
	{
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
		
		await base.OnLoadAsync();
	}

	protected override async Task<IEnumerable<EventInfoViewModel>> GetEventsInDateRange(DateTimeOffset startTime, DateTimeOffset endTime)
	{
		var eventResult = await ServerProxy!.GetCalendarEventsInPeriod(new GetCalendarEventsInPeriodRequest(CalendarId, startTime, endTime));

		Console.WriteLine(JsonSerializer.Serialize(eventResult, ApplicationJsonOptions.Options));

		if (!eventResult.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(eventResult.ErrorDescription!);
			return Array.Empty<EventInfoViewModel>();
		}
		
		EventInfoViewModel.AddCalendarName(CalendarId, CalendarName!);

		var events = eventResult.Value!
		   .Select(x => new EventInfoViewModel(x));

		return events;
	}
}