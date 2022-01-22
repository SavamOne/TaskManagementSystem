namespace TaskManagementSystem.BusinessLogic.Models;

public record Result<T>
{

    private Result(bool isSuccess, string? errorDescription, T? user)
    {
        IsSuccess = isSuccess;
        ErrorDescription = errorDescription;
        Value = user;
    }

    public bool IsSuccess { get; }

    public string? ErrorDescription { get; }

    public T? Value { get; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, null, value);
    }

    public static Result<T> Error(string description)
    {
        return new Result<T>(false, description, default);
    }
}