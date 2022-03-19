using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public class Temp_CalendarEventInfo
{
    public Temp_CalendarEventInfo(string name, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        Name = name.AssertNotNullOrWhiteSpace();
        StartTime = startTime;
        EndTime = endTime;
    }

    public string Name { get; }

    public DateTimeOffset StartTime { get; }

    public DateTimeOffset EndTime { get; }
}