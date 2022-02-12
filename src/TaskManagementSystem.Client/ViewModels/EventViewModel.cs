using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.ViewModels;

public record EventViewModel
{
    public EventViewModel() {}

    public EventViewModel(CalendarEventInfo eventInfo)
    {
        Name = eventInfo.Name;
        StartDate = eventInfo.StartTime;
        EndDate = eventInfo.EndTime;
    }

    public string? Name { get; set; }

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