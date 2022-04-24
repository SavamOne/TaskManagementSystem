using System.Globalization;
using TaskManagementSystem.Client.ViewModels;

namespace TaskManagementSystem.Client.Helpers.Implementations;

public static class DayOfWeekHelper
{
	public static ICollection<DayOfWeekViewModel> GetDayOfWeeksOrderedByFirstDay(CultureInfo cultureInfo, bool shortName)
	{
		return Enumerable.Range((int)cultureInfo.DateTimeFormat.FirstDayOfWeek, 7)
		   .Select(x => x % 7)
		   .Cast<DayOfWeek>()
		   .Select(x => new DayOfWeekViewModel(x, shortName, cultureInfo))
		   .ToList();
	}
}