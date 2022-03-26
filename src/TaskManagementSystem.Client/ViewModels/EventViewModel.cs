using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.ViewModels;

public record EventViewModel
{
	private bool changed;
	private string? description;
	private DateTimeOffset endDate = DateTimeOffset.Now.AddHours(1);
	private CalendarEventType eventType;
	private bool isPrivate;
	private string? name;
	private string? place;
	private DateTimeOffset startDate = DateTimeOffset.Now;

	public EventViewModel()
	{
		CanEditEvent = true;
		CanChangeParticipants = true;
		CanDeleteEvent = false;
	}

	public EventViewModel(EventInfo eventInfo,
		bool canEditEvent,
		bool canChangeParticipants,
		bool canDeleteEvent)
	{
		Id = eventInfo.Id;
		name = eventInfo.Name;
		place = eventInfo.Place;
		description = eventInfo.Description;
		startDate = eventInfo.StartTime.ToLocalTime();
		endDate = eventInfo.EndTime?.ToLocalTime() ?? DateTimeOffset.Now;
		isPrivate = eventInfo.IsPrivate;
		eventType = eventInfo.EventType;

		CanEditEvent = canEditEvent;
		CanChangeParticipants = canChangeParticipants;
		CanDeleteEvent = canDeleteEvent;
	}

	public bool Changed
	{
		get => changed;
		private set
		{
			changed = value;
			Console.WriteLine(value);
		}
	}

	public bool CanEditEvent { get; }

	public bool CanChangeParticipants { get; }

	public bool CanDeleteEvent { get; }

	public Guid Id { get; }

	public string? Name
	{
		get => name;
		set
		{
			name = value;
			Changed = true;
		}
	}

	public string? Description
	{
		get => description;
		set
		{
			description = value;
			Changed = true;
		}
	}

	public string? Place
	{
		get => place;
		set
		{
			place = value;
			Changed = true;
		}
	}

	public bool IsPrivate
	{
		get => isPrivate;
		set
		{
			isPrivate = value;
			Changed = true;
		}
	}

	public CalendarEventType EventType
	{
		get => eventType;
		set
		{
			eventType = value;
			Changed = true;
		}
	}

	public DateTimeOffset StartDate
	{
		get => startDate;
		set
		{
			startDate = value;
			Changed = true;
		}
	}

	public DateTimeOffset EndDate
	{
		get => endDate;
		set
		{
			endDate = value;
			Changed = true;
		}
	}

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
			IsPrivate);
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