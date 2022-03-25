using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.ViewModels;

public record EventViewModel
{
    public bool CanEditEvent { get; }

    public bool CanChangeParticipants { get; }

    public EventViewModel() {}

    public EventViewModel(EventInfo eventInfo, bool canEditEvent, bool canChangeParticipants)
    {
        Id = eventInfo.Id;
        Name = eventInfo.Name;
        Place = eventInfo.Place;
        Description = eventInfo.Description;
        StartDate = eventInfo.StartTime.ToLocalTime();
        EndDate = eventInfo.EndTime?.ToLocalTime() ?? DateTimeOffset.Now;
        IsPrivate = eventInfo.IsPrivate;
        EventType = eventInfo.EventType;
        
        CanEditEvent = canEditEvent;
        CanChangeParticipants = canChangeParticipants;
    }

    public Guid Id { get; }

    public string? Name { get; set; }
    
    public string? Description { get; set; }
    
    public string? Place { get; set; }
    
    public bool IsPrivate { get; set; }
    
    public CalendarEventType EventType { get; set; }
    
    public DateTimeOffset StartDate { get; set; } = DateTimeOffset.Now;

    public DateTimeOffset EndDate { get; set; } = DateTimeOffset.Now.AddHours(1);

    public string StartDateStr
    {
        get => StartDate.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");
        set
        {
            if (DateTimeOffset.TryParse(value, out DateTimeOffset date))
            {
                StartDate = date.ToOffset(DateTimeOffset.Now.Offset);
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

    public CreateEventRequest GetCreateRequest(Guid calendarId)
    {
        return new CreateEventRequest(calendarId,
            Name,
            Description,
            Place,
            EventType,
            StartDate,
            EndDate,
            false);
    }

    public EditEventRequest GetEditRequest()
    {
        return new EditEventRequest(Id,
            Name,
            Description,
            Place,
            EventType,
            StartDate,
            EndDate,
            IsPrivate);
    }
}