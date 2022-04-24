using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Shared.Models.Requests;

/// <summary>
///     Запрос на создание календаря.
/// </summary>
public record CreateCalendarRequest
{
	public CreateCalendarRequest(string name, string description)
	{
		Name = name;
		Description = description;
	}

	/// <summary>
	///     Имя календаря.
	/// </summary>
	[Required]
	public string Name { get; }

	/// <summary>
	///     Описание календаря.
	/// </summary>
	[Required]
	public string Description { get; }
}

/// <summary>
///     Запрос на редактирование календаря.
/// </summary>
public record EditCalendarRequest
{
	public EditCalendarRequest(Guid calendarId, string? name, string? description)
	{
		CalendarId = calendarId;
		Name = name;
		Description = description;
	}

	/// <summary>
	///     Id календаря.
	/// </summary>
	[Required]
	public Guid CalendarId { get; }

	/// <summary>
	///     Имя календаря.
	/// </summary>
	public string? Name { get; }

	/// <summary>
	///     Описание календаря.
	/// </summary>
	public string? Description { get; }
}

/// <summary>
///     Запрос на добавление участника календаря.
/// </summary>
public record AddCalendarParticipantRequest
{
	public AddCalendarParticipantRequest(Guid userId, CalendarParticipantRole role)
	{
		UserId = userId;
		Role = role;
	}

	/// <summary>
	///     Id пользователя.
	/// </summary>
	[Required]
	public Guid UserId { get; }

	/// <summary>
	///     Роль пользователя в календаре.
	/// </summary>
	[Required]
	public CalendarParticipantRole Role { get; }
}

/// <summary>
///     Запрос на добавление участников календаря.
/// </summary>
public record AddCalendarParticipantsRequest
{
	public AddCalendarParticipantsRequest(Guid calendarId, IEnumerable<AddCalendarParticipantRequest> users)
	{
		CalendarId = calendarId;
		Users = users;
	}

	/// <summary>
	///     Id календаря.
	/// </summary>
	[Required]
	public Guid CalendarId { get; }

	/// <summary>
	///     Коллекция <see cref="AddCalendarParticipantRequest" />.
	/// </summary>
	public IEnumerable<AddCalendarParticipantRequest> Users { get; }
}

/// <summary>
///     Запрос на изменение роли/удаление участника календаря.
/// </summary>
public record EditCalendarParticipantRequest
{
	public EditCalendarParticipantRequest(Guid participantId, CalendarParticipantRole? role, bool delete)
	{
		ParticipantId = participantId;
		Role = role;
		Delete = delete;
	}

	/// <summary>
	///     Id участника календаря.
	/// </summary>
	[Required]
	public Guid ParticipantId { get; }

	/// <summary>
	///     Роль участника.
	/// </summary>
	public CalendarParticipantRole? Role { get; }
	
	/// <summary>
	///     Флаг удаления.
	/// </summary>
	[Required]
	public bool Delete { get;  }
}

/// <summary>
///     Запрос на изменение роли/удаление участников календаря.
/// </summary>
public record EditCalendarParticipantsRequest
{
	public EditCalendarParticipantsRequest(Guid calendarId, IEnumerable<EditCalendarParticipantRequest> participants)
	{
		CalendarId = calendarId;
		Participants = participants;
	}

	/// <summary>
	///     Id календаря.
	/// </summary>
	[Required]
	public Guid CalendarId { get; }

	/// <summary>
	///     Коллекция <see cref="EditCalendarParticipantRequest" />.
	/// </summary>
	[Required]
	public IEnumerable<EditCalendarParticipantRequest> Participants { get; }
}

/// <summary>
///     Запрос на получение полной информации о календаре.
/// </summary>
public record GetCalendarInfoRequest
{
	public GetCalendarInfoRequest(Guid calendarId)
	{
		CalendarId = calendarId;
	}

	/// <summary>
	///     Id календаря.
	/// </summary>
	[Required]
	public Guid CalendarId { get; }
}

/// <summary>
///     Запрос на получение участников календаря по фильтру.
/// </summary>
public record GetCalendarParticipantsByFilterRequest
{
	public GetCalendarParticipantsByFilterRequest(Guid calendarId, string filter)
	{
		CalendarId = calendarId;
		Filter = filter;
	}

	/// <summary>
	///     Id календаря.
	/// </summary>
	[Required]
	public Guid CalendarId { get; }

	/// <summary>
	///     Фильтр имени или email.
	/// </summary>
	[Required]
	public string Filter { get; }
}

/// <summary>
///		Запрос на получение имен календарей.
/// </summary>
public record GetCalendarNamesRequest
{
	public GetCalendarNamesRequest(ISet<Guid> calendarIds) 
	{
		CalendarIds = calendarIds;
	}

	/// <summary>
	///     Id календаря.
	/// </summary>
	[Required]
	public ISet<Guid> CalendarIds { get; }
}

/// <summary>
///		Имя календаря.
/// </summary>
public record CalendarNameResponse
{
	public CalendarNameResponse(Guid calendarId, string name)
	{
		CalendarId = calendarId;
		Name = name;
	}

	/// <summary>
	///     Идентификатор календаря.
	/// </summary>
	[Required]
	public Guid CalendarId { get; }

	/// <summary>
	///     Имя.
	/// </summary>
	[Required]
	public string Name { get; }
}