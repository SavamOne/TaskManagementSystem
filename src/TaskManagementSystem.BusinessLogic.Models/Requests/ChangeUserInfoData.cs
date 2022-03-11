namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class ChangeUserInfoData
{
    public ChangeUserInfoData(Guid userId, string name, string email)
    {
        UserId = userId;
        Name = name;
        Email = email;
    }

    public Guid UserId { get; }

    public string Name { get; }

    public string Email { get; }
}