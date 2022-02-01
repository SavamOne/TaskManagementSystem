using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public class CalendarEvent
{
    public CalendarEvent(string name, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        Name = name.AssertNotNullOrWhiteSpace();
        StartTime = startTime;
        EndTime = endTime;
    }

    public string Name { get; }
    
    public DateTimeOffset StartTime { get; }
    
    public DateTimeOffset EndTime { get; }
}