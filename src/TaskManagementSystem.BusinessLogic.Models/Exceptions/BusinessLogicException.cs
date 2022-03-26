using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Exceptions;

public class BusinessLogicException : Exception
{
	public BusinessLogicException(string description)
	{
		Description = description.AssertNotNullOrWhiteSpace();
	}

	public BusinessLogicException(string description, params object?[] arguments)
	{
		description.AssertNotNullOrWhiteSpace();

		Description = string.Format(description, arguments);
	}

	public string Description { get; }

	public override string Message => Description;
}