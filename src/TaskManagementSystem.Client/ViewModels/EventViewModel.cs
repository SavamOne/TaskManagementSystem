using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.ViewModels;

public record EventViewModel
{
    private readonly ServerProxy serverProxy;
    
    public EventViewModel() {}

    public EventViewModel(EventInfo eventInfo)
    {
        Id = eventInfo.Id;
        Name = eventInfo.Name;
        Place = eventInfo.Place;
        Description = eventInfo.Description;
        StartDate = eventInfo.StartTime.ToLocalTime();
        EndDate = eventInfo.EndTime?.ToLocalTime() ?? DateTimeOffset.Now;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public string? Place { get; set; }
    

    public DateTimeOffset StartDate { get; set; } = DateTimeOffset.Now;

    public DateTimeOffset EndDate { get; set; } = DateTimeOffset.Now.AddHours(1);

    public string StartDateStr
    {
        get => StartDate.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");
        set
        {
            if (DateTimeOffset.TryParse(value, out DateTimeOffset date))
            {
                StartDate = date;
            }
        }
    }

    public string EndDateStr
    {
        get => EndDate.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");
        set
        {
            if (DateTimeOffset.TryParse(value, out DateTimeOffset date))
            {
                EndDate = date;
            }
        }
    }
}