namespace TaskManagementSystem.Shared.Dal.Extensions;

public static class DateTimeExtensions
{
	public static DateTime SetUtcKind(this DateTime dateTime)
	{
		return dateTime.Kind switch
		{
			DateTimeKind.Local => dateTime.ToUniversalTime(),
			DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
			DateTimeKind.Utc => dateTime,
			_ => throw new ArgumentOutOfRangeException(nameof(dateTime.Kind))
		};
	}
}