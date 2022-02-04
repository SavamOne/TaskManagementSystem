namespace TaskManagementSystem.BusinessLogic.Models;

public class RecurrentEventSettings
{
    public RecurrentEventSettings(Func<DateTime, DateTime> periodFunc, DateTime until)
    {
        PeriodFunc = periodFunc;
        Until = until;
    }

    public RecurrentEventSettings(Func<DateTime, DateTime> periodFunc, int eventCount)
    {
        PeriodFunc = periodFunc;
        EventCount = eventCount;
    }

    public RecurrentEventSettings(Func<DateTime, DateTime> periodFunc)
    {
        PeriodFunc = periodFunc;
    }

    public Func<DateTime, DateTime> PeriodFunc { get; }

    public bool Limit => EventCount is not null || Until is not null;

    public int? EventCount { get; }

    public DateTime? Until { get; }
}