using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Models;

public class User
{
	//TODO: DateJoined -> RegisterDate 
	public User(Guid id,
		string name,
		string email,
		string? position,
		string? department,
		DateTime registerDateUtc,
		byte[] passwordHash)
	{
		Name = name.AssertNotNullOrWhiteSpace();
		Email = email.AssertNotNullOrWhiteSpace();
		Id = id;
		Position = position;
		Department = department;
		RegisterDateUtc = registerDateUtc;
		PasswordHash = passwordHash.AssertNotNull();
	}

	public string Name { get; }

	public string Email { get; }

	public Guid Id { get; }

	public string? Position { get; }

	public string? Department { get; }

	public DateTime RegisterDateUtc { get; }

	public byte[] PasswordHash { get; }
}