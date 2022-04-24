using System.Text.Json;
using TaskManagementSystem.Client.ViewModels;
using TaskManagementSystem.Shared.Models.Options;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.Components;

public class CalendarEventWithUserEventsComponent : CalendarEventComponent
{
	protected override Task OnLoadAsync()
	{
		CalendarName = "События с вашим участием";

		return base.OnLoadAsync();
	}
	
	protected override async Task<IEnumerable<EventInfoViewModel>> GetEventsInDateRange(DateTimeOffset startTime, DateTimeOffset endTime)
	{
		var eventResult = await ServerProxy!.GetEventsInPeriodForUser(new GetEventsInPeriodForUserRequest(startTime, endTime));

		Console.WriteLine(JsonSerializer.Serialize(eventResult, ApplicationJsonOptions.Options));

		if (!eventResult.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(eventResult.ErrorDescription!);
			return Array.Empty<EventInfoViewModel>();
		}

		var unknownNames = EventInfoViewModel.GetUnknownCalendarNames(eventResult.Value!);
		if (unknownNames.Any())
		{
			var calendarNamesResult = await ServerProxy!.GetCalendarName(new GetCalendarNamesRequest(unknownNames));

			if (!calendarNamesResult.IsSuccess)
			{
				ToastService!.AddSystemErrorToast(calendarNamesResult.ErrorDescription!);
			}

			foreach (CalendarNameResponse nameResponse in calendarNamesResult.Value!)
			{
				EventInfoViewModel.AddCalendarName(nameResponse.CalendarId, nameResponse.Name);
			}
		}

		var events = eventResult.Value!
		   .Select(x => new EventInfoViewModel(x));

		return events;
	}
}