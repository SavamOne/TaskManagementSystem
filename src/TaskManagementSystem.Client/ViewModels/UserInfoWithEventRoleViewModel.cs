using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.ViewModels;

public class UserInfoWithEventRoleViewModel : UserInfoViewModel
{
	public UserInfoWithEventRoleViewModel(CalendarParticipantUser user)
		// HACK
		: base(new UserInfo(user.UserId, user.Username, user.Email, user.RegisterDate, string.Empty, string.Empty))
	{
		Role = EventParticipantRole.NotSet;
		CalendarParticipantId = user.Id;
	}

	public Guid CalendarParticipantId { get; }

	public EventParticipantRole Role { get; set; }

	public AddEventParticipantRequest GetAddParticipantRequest()
	{
		if (Role is EventParticipantRole.NotSet or EventParticipantRole.Creator)
		{
			throw new Exception();
		}

		return new AddEventParticipantRequest(CalendarParticipantId, Role);
	}
}