using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Shared.Models;

/// <summary>
///     Информация о событии.
/// </summary>
public class EventInfo
{
	public EventInfo(Guid id,
		Guid calendarId,
		string name,
		string? description,
		CalendarEventType eventType,
		string? place,
		DateTimeOffset startTime,
		DateTimeOffset endTime,
		bool isPrivate,
		DateTimeOffset creationTime,
		bool isRepeated,
		uint repeatNum)
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
		IsRepeated = isRepeated;
		RepeatNum = repeatNum;
	}

	/// <summary>
	///     Id события.
	/// </summary>
	[Required]
	public Guid Id { get; }

	/// <summary>
	///     Id календаря.
	/// </summary>
	[Required]
	public Guid CalendarId { get; }

	/// <summary>
	///     Имя события.
	/// </summary>
	[Required]
	public string Name { get; }

	/// <summary>
	///     Описание события.
	/// </summary>
	[Required]
	public string? Description { get; }

	/// <summary>
	///     Тип события.
	/// </summary>
	[Required]
	public CalendarEventType EventType { get; }

	/// <summary>
	///     Место события.
	/// </summary>
	[Required]
	public string? Place { get; }

	/// <summary>
	///     Время начала события.
	/// </summary>
	[Required]
	public DateTimeOffset StartTime { get; }

	/// <summary>
	///     Время окончания события.
	/// </summary>
	[Required]
	public DateTimeOffset EndTime { get; }

	/// <summary>
	///     Флаг конфиденциальности события.
	/// </summary>
	[Required]
	public bool IsPrivate { get; }

	/// <summary>
	///     Дата создания.
	/// </summary>
	[Required]
	public DateTimeOffset CreationTime { get; }

	/// <summary>
	///     Флаг, что для события заданы настройки повторения.
	/// </summary>
	[Required]
	public bool IsRepeated { get; }

	/// <summary>
	///     Номер повторения. Для события без повтора значение будет 0.
	/// </summary>
	[Required]
	public uint RepeatNum { get; }
}