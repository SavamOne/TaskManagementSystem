using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.ViewModels;

public record EventViewModel
{
	private string? description;
	private DateTimeOffset endDate = DateTimeOffset.Now.AddHours(1);
	private CalendarEventType eventType;
	private bool isPrivate;
	private string? name;
	private string? place;
	private DateTimeOffset startDate = DateTimeOffset.Now;
	private EventRepeatType repeatType;
	private ISet<DayOfWeek>? dayOfWeeks;

	public EventViewModel()
	{
		CanEditEvent = true;
		CanChangeParticipants = true;
		CanDeleteEvent = false;
	}
	
	public EventViewModel(EventInfo eventInfo)
		: this(eventInfo, null, false, false, false)
	{
	}
	
	public EventViewModel(EventInfo eventInfo,
		RecurrentSettings? recurrentSettings,
		bool canEditEvent,
		bool canChangeParticipants,
		bool canDeleteEvent)
	{
		Id = eventInfo.Id;
		name = eventInfo.Name;
		place = eventInfo.Place;
		description = eventInfo.Description;
		startDate = eventInfo.StartTime.ToLocalTime();
		endDate = eventInfo.EndTime.ToLocalTime();
		isPrivate = eventInfo.IsPrivate;
		eventType = eventInfo.EventType;
		repeatType = recurrentSettings?.RepeatType ?? EventRepeatType.None;
		dayOfWeeks = recurrentSettings?.DayOfWeeks;

		CanEditEvent = canEditEvent;
		CanChangeParticipants = canChangeParticipants;
		CanDeleteEvent = canDeleteEvent;
	}

	public bool Changed { get; private set; }

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
			endDate += value - startDate;
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

	public EventRepeatType RepeatType
	{
		get => repeatType;
		set
		{
			repeatType = value;
			Changed = true;
		}
	}

	public ISet<DayOfWeek> DayOfWeeks => dayOfWeeks ??= new HashSet<DayOfWeek>();

	public void CheckDayOfWeek(DayOfWeek dayOfWeek)
	{
		if (!DayOfWeeks.Contains(dayOfWeek))
		{
			DayOfWeeks.Add(dayOfWeek);
		}
		else
		{
			DayOfWeeks.Remove(dayOfWeek);
		}
		Changed = true;
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
		RecurrentSettings? recurrentSettings = repeatType is not EventRepeatType.None
			? new RecurrentSettings(repeatType, dayOfWeeks, uint.MaxValue, DateTimeOffset.MaxValue)
			: null;
		
		return new CreateEventRequest(calendarId,
			Name,
			Description,
			Place,
			EventType,
			StartDate,
			EndDate,
			IsPrivate,
			recurrentSettings);
	}

	public EditEventRequest GetEditRequest()
	{
		RecurrentSettings? recurrentSettings = repeatType is not EventRepeatType.None
			? new RecurrentSettings(repeatType, dayOfWeeks, UInt32.MaxValue, DateTimeOffset.MaxValue)
			: null;
		
		return new EditEventRequest(Id,
			recurrentSettings is not null,
			Name,
			Description,
			Place,
			EventType,
			StartDate,
			EndDate,
			IsPrivate,
			recurrentSettings);
	}
}