using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Models;

public class User
{
	//TODO: DateJoined -> RegisterDate 
	public User(Guid id,
		string name,
		string email,
		DateTime dateJoinedUtc,
		byte[] passwordHash)
	{
		Name = name.AssertNotNullOrWhiteSpace();
		Email = email.AssertNotNullOrWhiteSpace();
		Id = id;
		DateJoinedUtc = dateJoinedUtc;
		PasswordHash = passwordHash.AssertNotNull();
	}

	public string Name { get; }

	public string Email { get; }

	public Guid Id { get; }

	public DateTime DateJoinedUtc { get; }

	public byte[] PasswordHash { get; }
}