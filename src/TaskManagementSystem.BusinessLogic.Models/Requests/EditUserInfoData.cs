namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class EditUserInfoData
{
	public EditUserInfoData(Guid userId, string? name, string? email, string? position, string? department)
	{
		UserId = userId;
		Name = name;
		Email = email;
		Position = position;
		Department = department;
	}

	public Guid UserId { get; }

	public string? Name { get; }

	public string? Email { get; }

	public string? Position { get; }

	public string? Department { get; }
}