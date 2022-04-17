using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Shared.Models.Requests;

/// <summary>
///     Запрос на создание календаря.
/// </summary>
public class CreateCalendarRequest
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
public class EditCalendarRequest
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
public class AddCalendarParticipantRequest
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
public class AddCalendarParticipantsRequest
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
///     Запрос на изменение роли участника календаря.
/// </summary>
public class ChangeCalendarParticipantRoleRequest
{
	public ChangeCalendarParticipantRoleRequest(Guid participantId, CalendarParticipantRole role)
	{
		ParticipantId = participantId;
		Role = role;
	}

	/// <summary>
	///     Id участника календаря.
	/// </summary>
	[Required]
	public Guid ParticipantId { get; }

	/// <summary>
	///     Роль участника.
	/// </summary>
	[Required]
	public CalendarParticipantRole Role { get; }
}

/// <summary>
///     Запрос на изменение ролей участников календаря.
/// </summary>
public class ChangeCalendarParticipantsRoleRequest
{
	public ChangeCalendarParticipantsRoleRequest(Guid calendarId, IEnumerable<ChangeCalendarParticipantRoleRequest> participants)
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
	///     Коллекция <see cref="ChangeCalendarParticipantsRoleRequest" />.
	/// </summary>
	[Required]
	public IEnumerable<ChangeCalendarParticipantRoleRequest> Participants { get; }
}

/// <summary>
///     Запрос на удаление участников календаря.
/// </summary>
public class DeleteParticipantsRequest
{
	public DeleteParticipantsRequest(Guid calendarId, IEnumerable<Guid> participantIds)
	{
		CalendarId = calendarId;
		ParticipantIds = participantIds;
	}

	/// <summary>
	///     Id календаря.
	/// </summary>
	[Required]
	public Guid CalendarId { get; }

	/// <summary>
	///     Коллекция Id участников календаря.
	/// </summary>
	[Required]
	public IEnumerable<Guid> ParticipantIds { get; }
}

/// <summary>
///     Запрос на получение полной информации о календаре.
/// </summary>
public class GetCalendarInfoRequest
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
public class GetCalendarParticipantsByFilterRequest
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
public record GetCalendarNameRequest
{
	public GetCalendarNameRequest(ISet<Guid> calendarIds) 
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