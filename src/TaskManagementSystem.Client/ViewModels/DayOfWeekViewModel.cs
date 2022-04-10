using System.Globalization;

namespace TaskManagementSystem.Client.ViewModels;

public class DayOfWeekViewModel
{
	private readonly bool shortName;
	private readonly CultureInfo cultureInfo;

	public DayOfWeekViewModel(DayOfWeek firstDay, bool shortName, CultureInfo cultureInfo)
	{
		Value = firstDay;
		this.shortName = shortName;
		this.cultureInfo = cultureInfo;
	}

	public DayOfWeek Value { get; }

	public override string ToString()
	{
		string name = shortName ? 
			cultureInfo.DateTimeFormat.GetShortestDayName(Value) : 
			cultureInfo.DateTimeFormat.GetDayName(Value);
		
		return cultureInfo.TextInfo.ToTitleCase(name);
	}
}