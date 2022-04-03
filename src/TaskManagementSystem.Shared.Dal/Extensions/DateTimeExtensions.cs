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

	public static DateTime StripSeconds(this DateTime dateTime)
	{
		return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Kind);
	}
}