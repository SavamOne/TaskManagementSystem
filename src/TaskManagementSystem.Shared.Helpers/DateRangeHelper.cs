namespace TaskManagementSystem.Shared.Helpers;

public static class DateRangeHelper
{
    public static bool IsEventInCalendarRange(DateTimeOffset eventStartTime, DateTimeOffset eventEndTime, DateTimeOffset calendarStartTime, DateTimeOffset calendarEndTime)
    {
        return eventStartTime >= calendarStartTime && eventStartTime <= calendarEndTime
               || eventEndTime >= calendarStartTime && eventEndTime <= calendarEndTime
               || eventStartTime <= calendarStartTime && eventEndTime >= calendarEndTime;
    }

    public static bool IsEventInCalendarRange(DateTime eventStartTime, DateTime eventEndTime, DateTime calendarStartTime, DateTime calendarEndTime)
    {
        return eventStartTime >= calendarStartTime && eventStartTime <= calendarEndTime
               || eventEndTime >= calendarStartTime && eventEndTime <= calendarEndTime
               || eventStartTime <= calendarStartTime && eventEndTime >= calendarEndTime;
    }

    public static bool IsEventInCalendarDay(DateTimeOffset eventStartTime, DateTimeOffset eventEndTime, DateOnly calendarDay)
    {
        DateTimeOffset startTime = calendarDay.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local);
        DateTimeOffset endTime = calendarDay.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Local);

        return IsEventInCalendarRange(eventStartTime, eventEndTime, startTime, endTime);
    }
}