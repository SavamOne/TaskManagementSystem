using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.ViewModels;

public record EventViewModel
{
    public EventViewModel() {}

    public EventViewModel(EventInfo eventInfo)
    {
        Name = eventInfo.Name;
        Place = eventInfo.Place;
        Description = eventInfo.Description;
        StartDate = eventInfo.StartTime.ToLocalTime();
        EndDate = eventInfo.EndTime?.ToLocalTime() ?? DateTimeOffset.Now;
    }

    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public string? Place { get; set; }
    

    public DateTimeOffset StartDate { get; set; }

    public DateTimeOffset EndDate { get; set; }

    public string StartDateStr
    {
        get => StartDate.ToLocalTime().ToString("yyyy-MM-ddThh:mm");
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
        get => EndDate.ToLocalTime().ToString("yyyy-MM-ddThh:mm");
        set
        {
            if (DateTimeOffset.TryParse(value, out DateTimeOffset date))
            {
                EndDate = date;
            }
        }
    }
}