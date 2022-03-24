using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.ViewModels;

public class UserInfoWithEventRoleViewModel  : UserInfoViewModel
{
	public UserInfoWithEventRoleViewModel(CalendarParticipantUser user) 
		: base(new UserInfo(user.UserId, user.Username, user.Email, user.RegisterDate))
	{
		
	}
	
	public EventParticipantRole Role { get; set; }

	public AddEventParticipantRequest GetAddParticipantRequest()
	{
		if (Role is EventParticipantRole.NotSet or EventParticipantRole.Creator)
		{
			throw new Exception();
		}

		return new AddEventParticipantRequest(UserId, Role);
	}
}