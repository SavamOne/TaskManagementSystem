using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public class CalendarEventInfo
{
    public CalendarEventInfo(string name, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        Name = name.AssertNotNullOrWhiteSpace();
        StartTime = startTime;
        EndTime = endTime;
    }

    public string Name { get; }

    public DateTimeOffset StartTime { get; }

    public DateTimeOffset EndTime { get; }
}