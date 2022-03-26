using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public record Result<T>
{
	private Result(string errorDescription)
	{
		ErrorDescription = errorDescription.AssertNotNull();
	}

	private Result(T value)
	{
		Value = value.AssertNotNull();
	}

	public bool IsSuccess => ErrorDescription is null;

	public string? ErrorDescription { get; }

	public T? Value { get; }

	public static Result<T> Success(T value)
	{
		return new Result<T>(value);
	}

	public static Result<T> Error(string description)
	{
		return new Result<T>(description);
	}
}