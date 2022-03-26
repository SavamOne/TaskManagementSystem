namespace TaskManagementSystem.Shared.Models;

public class EventInfo
{
	public EventInfo(Guid id,
		Guid calendarId,
		string name,
		string? description,
		CalendarEventType eventType,
		string? place,
		DateTimeOffset startTime,
		DateTimeOffset? endTime,
		bool isPrivate,
		DateTimeOffset creationTime)
	{
		Id = id;
		CalendarId = calendarId;
		Name = name;
		Description = description;
		EventType = eventType;
		Place = place;
		StartTime = startTime;
		EndTime = endTime;
		IsPrivate = isPrivate;
		CreationTime = creationTime;
	}

	public Guid Id { get; }

	public Guid CalendarId { get; }

	public string Name { get; }

	public string? Description { get; }

	public CalendarEventType EventType { get; }

	public string? Place { get; }

	public DateTimeOffset StartTime { get; }

	public DateTimeOffset? EndTime { get; }

	public bool IsPrivate { get; }

	public DateTimeOffset CreationTime { get; }
}