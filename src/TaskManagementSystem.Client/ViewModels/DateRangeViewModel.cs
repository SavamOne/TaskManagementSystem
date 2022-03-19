using System.Text.Json;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Options;

namespace TaskManagementSystem.Client.ViewModels;

public class DateRangeViewModel
{
    private readonly ServerProxy serverProxy;
    private readonly IToastService toastService;
    private readonly Guid calendarId;

    public DateRangeViewModel(ServerProxy serverProxy, IToastService toastService, IEnumerable<DayViewModel> days, Guid calendarId)
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
        var result = await serverProxy.GetEventsInPeriod(new GetEventsInPeriodRequest(calendarId, FirstDay.DateTimeOffset, LastDay.DateTimeOffset.AddDays(1)));

        Console.WriteLine(JsonSerializer.Serialize(result, ApplicationJsonOptions.Options));

        if (!result.IsSuccess)
        {
            toastService.AddSystemErrorToast(result.ErrorDescription!);
        }

        var events = result.Value!.Select(x => new EventViewModel(x)).ToList();

        foreach (DayViewModel dayViewModel in Days)
        {
            dayViewModel.InterceptDateEvents(events);
            events.RemoveAll(x => x.EndDate < dayViewModel.DateTimeOffset.AddDays(1));
        }
    }

    public void ClearEvents()
    {
        foreach (DayViewModel dayViewModel in Days)
        {
            dayViewModel.ClearEvents();
        }
    }
}