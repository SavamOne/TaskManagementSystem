using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.ViewModels;

public class UserInfoWithParticipantRoleViewModel : UserInfoViewModel
{
	public UserInfoWithParticipantRoleViewModel(UserInfo userInfo)
		: base(userInfo)
	{
		Role = CalendarParticipantRole.NotSet;
	}

	public CalendarParticipantRole Role { get; set; }

	public AddCalendarParticipantRequest GetAddParticipantRequest()
	{
		if (Role is CalendarParticipantRole.NotSet or CalendarParticipantRole.Creator)
		{
			throw new Exception();
		}

		return new AddCalendarParticipantRequest(UserId, Role);
	}
}