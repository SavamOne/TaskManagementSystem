using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

/// <summary>
///     Полная информация о событии вместе с участниками.
/// </summary>
public class CalendarWithParticipantUsers
{
	public CalendarWithParticipantUsers(CalendarInfo calendar, IEnumerable<CalendarParticipantUser> participants)
	{
		Calendar = calendar.AssertNotNull();
		Participants = participants.AssertNotNull();
	}

	/// <summary>
	///     Информация о календаре.
	/// </summary>
	[Required]
	public CalendarInfo Calendar { get; }

	/// <summary>
	///     Коллекция участников.
	/// </summary>
	[Required]
	public IEnumerable<CalendarParticipantUser> Participants { get; }
}