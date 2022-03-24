using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Extensions;

public static class CalendarParticipantRoleExtensions
{
	private static readonly ICollection<CalendarRole> Roles = Enum.GetValues<CalendarRole>();

	public static bool IsAdminOrCreator(this CalendarParticipant? calendarParticipant)
	{
		return calendarParticipant != null && IsAdminOrCreator(calendarParticipant.Role);
	}
	
	public static bool IsAdminOrCreator(this CalendarRole role)
	{
		return role is CalendarRole.Admin or CalendarRole.Creator;
	}
	
	public static bool IsCreator(this CalendarRole role)
	{
		return role is CalendarRole.Creator;
	}

	public static bool IsAdminOrParticipant(this CalendarRole role)
	{
		return role is CalendarRole.Admin or CalendarRole.Participant;
	}

}