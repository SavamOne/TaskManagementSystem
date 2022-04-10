using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class AddCalendarEventData
{
	public AddCalendarEventData(Guid userId,
		Guid calendarId,
		string name,
		string? description,
		EventType eventType,
		string? place,
		DateTimeOffset startTime,
		DateTimeOffset endTime,
		bool isPrivate,
		AddRecurrentSettingsData? recurrentSettingsData)
	{
		UserId = userId;
		CalendarId = calendarId;
		Name = name.AssertNotNull();
		Description = description;
		EventType = eventType;
		Place = place;
		StartTime = startTime;
		EndTime = endTime;
		IsPrivate = isPrivate;
		RecurrentSettingsData = recurrentSettingsData;
	}

	public Guid UserId { get; }

	public Guid CalendarId { get; }

	public string Name { get; }

	public string? Description { get; }

	public EventType EventType { get; }

	public string? Place { get; }

	public DateTimeOffset StartTime { get; }

	public DateTimeOffset EndTime { get; }

	public bool IsPrivate { get; }
	
	public AddRecurrentSettingsData? RecurrentSettingsData { get; }
}