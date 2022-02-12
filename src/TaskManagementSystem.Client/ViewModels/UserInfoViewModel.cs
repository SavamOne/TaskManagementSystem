using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.ViewModels;

public class UserInfoViewModel
{
    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? RegisterDate { get; set; }

    public void SetInfo(UserInfo userInfo)
    {
        Name = userInfo.Name;
        Email = userInfo.Email;
        RegisterDate = userInfo.DateJoined.ToLocalTime().ToString("d");
    }

    public ChangeUserInfoRequest GetRequest()
    {
        return new ChangeUserInfoRequest(Name!, Email!);
    }
}