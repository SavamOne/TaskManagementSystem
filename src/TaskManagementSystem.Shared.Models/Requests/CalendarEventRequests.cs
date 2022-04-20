using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Shared.Models.Requests;

/// <summary>
///     Запрос на создание события.
/// </summary>
public class CreateEventRequest
{
	public CreateEventRequest(Guid calendarId,
		string name,
		string? description,
		string? place,
		CalendarEventType type,
		DateTimeOffset startTime,
		DateTimeOffset endTime,
		bool isPrivate,
		RecurrentSettings? recurrentSettings)
	{
		CalendarId = calendarId;
		Name = name;
		Description = description;
		Place = place;
		Type = type;
		StartTime = startTime;
		EndTime = endTime;
		IsPrivate = isPrivate;
		RecurrentSettings = recurrentSettings;
	}

	/// <summary>
	///     Id календаря.
	/// </summary>
	[Required]
	public Guid CalendarId { get; }

	/// <summary>
	///     Имя.
	/// </summary>
	[Required]
	public string Name { get; }

	/// <summary>
	///     Описание.
	/// </summary>
	[Required]
	public string? Description { get; }

	/// <summary>
	///     Место события.
	/// </summary>
	[Required]
	public string? Place { get; }

	/// <summary>
	///     Тип события.
	/// </summary>
	[Required]
	public CalendarEventType Type { get; }

	/// <summary>
	///     Время начала.
	/// </summary>
	[Required]
	public DateTimeOffset StartTime { get; }

	/// <summary>
	///     Время окончания.
	/// </summary>
	[Required]
	public DateTimeOffset EndTime { get; }

	/// <summary>
	///     Флаг конфиденциальности.
	/// </summary>
	[Required]
	public bool IsPrivate { get; }

	/// <summary>
	///     Настройки повторения событий.
	/// </summary>
	public RecurrentSettings? RecurrentSettings { get; }
}

/// <summary>
///     Запрос на редактирование события.
/// </summary>
public class EditEventRequest
{
	public EditEventRequest(Guid eventId,
		bool isRepeated,
		string? name,
		string? description,
		string? place,
		CalendarEventType? type,
		DateTimeOffset? startTime,
		DateTimeOffset? endTime,
		bool? isPrivate,
		RecurrentSettings? recurrentSettings)
	{
		EventId = eventId;
		IsRepeated = isRepeated;
		Name = name;
		Description = description;
		Place = place;
		Type = type;
		StartTime = startTime;
		EndTime = endTime;
		IsPrivate = isPrivate;
		RecurrentSettings = recurrentSettings;
	}

	/// <summary>
	///     Id события.
	/// </summary>
	[Required]
	public Guid EventId { get; }

	/// <summary>
	///     Флаг повторения события.
	/// </summary>
	[Required]
	public bool IsRepeated { get; }

	/// <summary>
	///     Имя.
	/// </summary>
	public string? Name { get; }

	/// <summary>
	///     Описание.
	/// </summary>
	public string? Description { get; }

	/// <summary>
	///     Место события.
	/// </summary>
	public string? Place { get; }

	/// <summary>
	///     Тип события.
	/// </summary>
	public CalendarEventType? Type { get; }

	/// <summary>
	///     Время начала.
	/// </summary>
	public DateTimeOffset? StartTime { get; }

	/// <summary>
	///     Время окончания.
	/// </summary>
	public DateTimeOffset? EndTime { get; }

	/// <summary>
	///     Флаг конфиденциальности.
	/// </summary>
	public bool? IsPrivate { get; }

	/// <summary>
	///    Настройки повторения события. Учитывается только если <see cref="IsRepeated"/> истина.
	/// </summary>
	public RecurrentSettings? RecurrentSettings { get; }
}

/// <summary>
///     Запрос на удаление события.
/// </summary>
public class DeleteEventRequest
{
	public DeleteEventRequest(Guid eventId)
	{
		EventId = eventId;
	}

	/// <summary>
	///     Id события.
	/// </summary>
	[Required]
	public Guid EventId { get; }
}

/// <summary>
///     Запрос на добавление участника события.
/// </summary>
public class AddEventParticipantRequest
{
	public AddEventParticipantRequest(Guid participantId, EventParticipantRole role)
	{
		ParticipantId = participantId;
		Role = role;
	}

	/// <summary>
	///     Id участника события.
	/// </summary>
	[Required]
	public Guid ParticipantId { get; }

	/// <summary>
	///     Роль участника.
	/// </summary>
	[Required]
	public EventParticipantRole Role { get; }
}

/// <summary>
///     Запрос на добавление участников события.
/// </summary>
public class AddEventParticipantsRequest
{
	public AddEventParticipantsRequest(Guid eventId, ICollection<AddEventParticipantRequest> participants)
	{
		EventId = eventId;
		Participants = participants;
	}

	/// <summary>
	///     Id события.
	/// </summary>
	[Required]
	public Guid EventId { get; }

	/// <summary>
	///     Коллекция <see cref="AddEventParticipantRequest" />.
	/// </summary>
	[Required]
	public ICollection<AddEventParticipantRequest> Participants { get; }
}

/// <summary>
///     Запрос на изменение роли/удаление участника события.
/// </summary>
public class ChangeEventParticipantRequest
{
	public ChangeEventParticipantRequest(Guid eventParticipantId, EventParticipantRole? role, bool delete)
	{
		EventParticipantId = eventParticipantId;
		Role = role;
		Delete = delete;
	}

	/// <summary>
	///     Id участника события.
	/// </summary>
	[Required]
	public Guid EventParticipantId { get; }

	/// <summary>
	///     Роль участника.
	/// </summary>
	public EventParticipantRole? Role { get; }

	/// <summary>
	///     Флаг удаления.
	/// </summary>
	[Required]
	public bool Delete { get; }
}

/// <summary>
///     Запрос на изменение роли/удаление участников события.
/// </summary>
public class ChangeEventParticipantsRequest
{
	public ChangeEventParticipantsRequest(Guid eventId, ICollection<ChangeEventParticipantRequest> participants)
	{
		EventId = eventId;
		Participants = participants;
	}

	/// <summary>
	///     Id события.
	/// </summary>
	[Required]
	public Guid EventId { get; }

	/// <summary>
	///     Коллекция <see cref="ChangeEventParticipantRequest" />.
	/// </summary>
	[Required]
	public ICollection<ChangeEventParticipantRequest> Participants { get; }
}

/// <summary>
///     Запрос на получение полной информации о событии.
/// </summary>
public class GetEventInfoRequest
{
	public GetEventInfoRequest(Guid eventId)
	{
		EventId = eventId;
	}

	/// <summary>
	///     Id события.
	/// </summary>
	[Required]
	public Guid EventId { get; }
}

/// <summary>
///     Запрос на получения списка событий в периоде в конкретном календаре.
/// </summary>
public class GetCalendarEventsInPeriodRequest
{
	public GetCalendarEventsInPeriodRequest(Guid calendarId, DateTimeOffset startPeriod, DateTimeOffset endPeriod)
	{
		CalendarId = calendarId;
		StartPeriod = startPeriod;
		EndPeriod = endPeriod;
	}

	/// <summary>
	///     Id календаря.
	/// </summary>
	[Required]
	public Guid CalendarId { get; }

	/// <summary>
	///     Начальный период.
	/// </summary>
	[Required]
	public DateTimeOffset StartPeriod { get; }

	/// <summary>
	///     Конечный период.
	/// </summary>
	[Required]
	public DateTimeOffset EndPeriod { get; }
}

/// <summary>
///     Запрос на получения списка событий для пользователя в периоде.
/// </summary>
public class GetEventsInPeriodForUserRequest
{
	public GetEventsInPeriodForUserRequest(DateTimeOffset startPeriod, DateTimeOffset endPeriod)
	{
		StartPeriod = startPeriod;
		EndPeriod = endPeriod;
	}
	
	/// <summary>
	///     Начальный период.
	/// </summary>
	[Required]
	public DateTimeOffset StartPeriod { get; }

	/// <summary>
	///     Конечный период.
	/// </summary>
	[Required]
	public DateTimeOffset EndPeriod { get; }
}

/// <summary>
///     Запрос на обновление состояния участия в событии.
/// </summary>
public class ChangeMyEventParticipationStateRequest
{
	public ChangeMyEventParticipationStateRequest(Guid eventId, EventParticipantState participantState, TimeSpan? notifyBefore)
	{
		EventId = eventId;
		ParticipantState = participantState;
		NotifyBefore = notifyBefore;
	}
	
	/// <summary>
	///     Идентификатор события.
	/// </summary>
	[Required]
	public Guid EventId { get; }

	/// <summary>
	///     Состояние участия в событии.
	/// </summary>
	[Required]
	public EventParticipantState ParticipantState { get; }
	
	/// <summary>
	///     Время, за которое необходимо напомнить о событии.
	/// </summary>
	public TimeSpan? NotifyBefore { get; }
}