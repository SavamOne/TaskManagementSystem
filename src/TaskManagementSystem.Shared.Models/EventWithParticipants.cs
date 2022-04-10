using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

/// <summary>
///     Полная информация о событии вместе с участниками.
/// </summary>
public class EventWithParticipants
{
	public EventWithParticipants(EventInfo eventInfo,
		ICollection<EventParticipantUser> participants,
		bool canUserEditEvent,
		bool canUserEditParticipants,
		bool canUserDeleteEvent,
		RecurrentSettings? recurrentSettings)
	{
		EventInfo = eventInfo.AssertNotNull();
		Participants = participants.AssertNotNull();
		CanUserEditEvent = canUserEditEvent;
		CanUserEditParticipants = canUserEditParticipants;
		CanUserDeleteEvent = canUserDeleteEvent;
		RecurrentSettings = recurrentSettings;
	}

	/// <summary>
	///     Флаг права на редактирование события у пользователя.
	/// </summary>
	[Required]
	public bool CanUserEditEvent { get; }

	/// <summary>
	///     Флаг права на редактирование списка участников у пользователя.
	/// </summary>
	[Required]
	public bool CanUserEditParticipants { get; }

	/// <summary>
	///     Флаг права на удаление события у пользователя
	/// </summary>
	[Required]
	public bool CanUserDeleteEvent { get; }

	/// <summary>
	///     Информация о событии.
	/// </summary>
	[Required]
	public EventInfo EventInfo { get; }

	/// <summary>
	///     Коллекция участников.
	/// </summary>
	[Required]
	public ICollection<EventParticipantUser> Participants { get; }
	
	/// <summary>
	///     Настройки повторения.
	/// </summary>
	public RecurrentSettings? RecurrentSettings { get; }
}