using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.ViewModels;

public class UserInfoWithRoleViewModel : UserInfoViewModel
{
    public UserInfoWithRoleViewModel(UserInfo userInfo) : base(userInfo)
    {
        Role = CalendarParticipantRole.NotSet;
    }
    
    public CalendarParticipantRole Role { get; set; }

    public AddCalendarParticipantRequest GetAddParticipantRequest()
    {
        if (Role == CalendarParticipantRole.NotSet)
        {
            throw new Exception();
        }
        
        return new AddCalendarParticipantRequest(UserId, Role);
    }
}