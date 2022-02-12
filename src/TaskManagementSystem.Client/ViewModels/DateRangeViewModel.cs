using System.Text.Json;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Options;

namespace TaskManagementSystem.Client.ViewModels;

public class DateRangeViewModel
{
    private readonly ServerProxy serverProxy;

    public DateRangeViewModel(ServerProxy serverProxy, IEnumerable<DayViewModel> days)
    {
        this.serverProxy = serverProxy;
        Days = days;
    }

    public IEnumerable<DayViewModel> Days { get; }

    public DayViewModel FirstDay => Days.First();

    public DayViewModel LastDay => Days.Last();

    public async Task GetEventsAsync()
    {
        var result = await serverProxy.GetEventsForMonth(new CalendarGetEventsRequest(FirstDay.DateTimeOffset, LastDay.DateTimeOffset.AddDays(1)));

        Console.WriteLine(JsonSerializer.Serialize(result, ApplicationJsonOptions.Options));

        if (!result.IsSuccess)
        {
            //TODO: Не бросать исключение.
            throw new Exception(result.ErrorDescription);
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