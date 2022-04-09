using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Models;

public record CalendarEvent
{
	public CalendarEvent(Guid id,
		Guid calendarId,
		string name,
		string? description,
		EventType eventType,
		string? place,
		DateTime startTimeUtc,
		DateTime endTimeUtc,
		bool isPrivate,
		DateTime creationTimeUtc,
		bool isRepeated)
	{
		Id = id;
		CalendarId = calendarId;
		Name = name.AssertNotNullOrWhiteSpace();
		Description = description;
		EventType = eventType;
		Place = place;
		StartTimeUtc = startTimeUtc;
		EndTimeUtc = endTimeUtc;
		IsPrivate = isPrivate;
		CreationTimeUtc = creationTimeUtc;
		IsRepeated = isRepeated;
	}

	public Guid Id { get; }

	public Guid CalendarId { get; }

	public string Name { get; set; }

	public string? Description { get; set; }

	public EventType EventType { get; set; }

	public string? Place { get; set; }

	public DateTime StartTimeUtc { get; set; }

	public DateTime EndTimeUtc { get; set; }

	public bool IsPrivate { get; set; }

	public DateTime CreationTimeUtc { get; }
	
	public bool IsRepeated { get; set; }
	
	public uint RepeatNum { get; set; }
}