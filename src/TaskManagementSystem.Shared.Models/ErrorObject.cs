using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public class ErrorObject
{
	public ErrorObject(string error)
	{
		Error = error.AssertNotNull();
	}

	public string Error { get; }
}