using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.ViewModels;

public class CalendarParticipantViewModel
{

	private readonly CalendarParticipantUser calendarParticipantUser;
	private CalendarParticipantRole role;

	public CalendarParticipantViewModel(CalendarParticipantUser calendarParticipantUser)
	{
		this.calendarParticipantUser = calendarParticipantUser;

		role = calendarParticipantUser.Role;
		UserId = calendarParticipantUser.UserId;
		CalendarParticipantId = calendarParticipantUser.Id;
		Name = calendarParticipantUser.Username;
		Email = calendarParticipantUser.Email;
		RegisterDate = calendarParticipantUser.RegisterDate.ToLocalTime().ToString("d");
		CalendarJoinDate = calendarParticipantUser.CalendarJoinDate.ToLocalTime().ToString("d");
	}

	public bool RoleChanged { get; private set; }

	public Guid UserId { get; }

	public Guid CalendarParticipantId { get; }

	public string Name { get; }

	public string Email { get; }

	public string RegisterDate { get; }

	public string CalendarJoinDate { get; }

	public CalendarParticipantRole Role
	{
		get => role;
		set
		{
			RoleChanged = true;
			role = value;
		}
	}

	public EditCalendarParticipantRequest GetChangeRoleRequest()
	{
		if (!RoleChanged)
		{
			throw new Exception();
		}

		return new EditCalendarParticipantRequest(CalendarParticipantId, 
			Role is not CalendarParticipantRole.NotSet ? Role : null, 
			Role is CalendarParticipantRole.NotSet);
	}
}