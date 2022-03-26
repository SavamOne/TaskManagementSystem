namespace TaskManagementSystem.BusinessLogic.Models.Models;

public class Temp_RecurrentEventSettings
{
	public Temp_RecurrentEventSettings(Func<DateTime, DateTime> periodFunc, DateTime until)
	{
		PeriodFunc = periodFunc;
		Until = until;
	}

	public Temp_RecurrentEventSettings(Func<DateTime, DateTime> periodFunc, int eventCount)
	{
		PeriodFunc = periodFunc;
		EventCount = eventCount;
	}

	public Temp_RecurrentEventSettings(Func<DateTime, DateTime> periodFunc)
	{
		PeriodFunc = periodFunc;
	}

	public Func<DateTime, DateTime> PeriodFunc { get; }

	public bool Limit => EventCount is not null || Until is not null;

	public int? EventCount { get; }

	public DateTime? Until { get; }
}