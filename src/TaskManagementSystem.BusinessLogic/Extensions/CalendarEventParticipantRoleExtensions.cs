using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Extensions;

public static class CalendarEventParticipantRoleExtensions
{
	public static bool IsParticipantOrCreator(this CalendarEventParticipant? eventParticipant)
	{
		return eventParticipant != null && IsParticipantOrCreator(eventParticipant.Role);
	}

	public static bool IsParticipantOrCreator(this CalendarEventParticipantRole role)
	{
		return role is CalendarEventParticipantRole.Participant or CalendarEventParticipantRole.Creator;
	}
	
	public static bool IsParticipant(this CalendarEventParticipant? eventParticipant)
	{
		return eventParticipant?.Role is CalendarEventParticipantRole.Participant;
	}

	public static bool IsParticipantOrInform(this CalendarEventParticipantRole role)
	{
		return role is CalendarEventParticipantRole.Participant or CalendarEventParticipantRole.Inform;
	}

	public static bool IsCreator(this CalendarEventParticipant? eventParticipant)
	{
		return eventParticipant != null && IsCreator(eventParticipant.Role);
	}

	public static bool IsCreator(this CalendarEventParticipantRole role)
	{
		return role is CalendarEventParticipantRole.Creator;
	}
}