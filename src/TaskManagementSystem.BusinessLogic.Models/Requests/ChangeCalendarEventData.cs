using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class ChangeCalendarEventData
{
	public ChangeCalendarEventData(Guid userId,
		Guid eventId,
		bool isRepeated,
		string? name,
		string? description,
		EventType? eventType,
		string? place,
		DateTimeOffset? startTime,
		DateTimeOffset? endTime,
		bool? isPrivate,
		AddRecurrentSettingsData? recurrentSettingsData)
	{
		UserId = userId;
		EventId = eventId;
		Name = name;
		Description = description;
		EventType = eventType;
		Place = place;
		StartTime = startTime;
		EndTime = endTime;
		IsPrivate = isPrivate;
		IsRepeated = isRepeated;
		RecurrentSettingsData = recurrentSettingsData;
	}

	public Guid UserId { get; }

	public Guid EventId { get; }

	public string? Name { get; }

	public string? Description { get; }

	public EventType? EventType { get; }

	public string? Place { get; }

	public DateTimeOffset? StartTime { get; }

	public DateTimeOffset? EndTime { get; }

	public bool? IsPrivate { get; }
	
	public bool IsRepeated { get; }
	
	public AddRecurrentSettingsData? RecurrentSettingsData { get; }
}