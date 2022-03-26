using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public class UserInfo
{
	public UserInfo(Guid id,
		string name,
		string email,
		DateTimeOffset dateJoined)
	{
		Id = id;
		Name = name.AssertNotNullOrWhiteSpace();
		Email = email.AssertNotNullOrWhiteSpace();
		DateJoined = dateJoined;
	}

	public Guid Id { get; }

	public string Name { get; }

	public string Email { get; }

	public DateTimeOffset DateJoined { get; }
}