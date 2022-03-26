using System.Globalization;

namespace TaskManagementSystem.Client.Services;

public interface ILocalizationService
{
	Task InitializeAsync();

	Task<CultureInfo> GetApplicationCultureAsync();

	Task<DateTimeFormatInfo> GetApplicationDateTimeFormatAsync();

	Task<string> GeApplicationCultureNameAsync();

	Task<DayOfWeek> GetApplicationFirstDayOfWeekAsync();

	Task SetApplicationCultureAsync(CultureInfo culture);

	Task SetApplicationFirstDayOfWeekAsync(DayOfWeek dayOfWeek);

	IEnumerable<CultureInfo> GetAvailableCultures();
}