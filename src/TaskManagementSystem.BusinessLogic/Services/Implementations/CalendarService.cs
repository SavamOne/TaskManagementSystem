using System.Diagnostics;
using System.Globalization;
using Ical.Net;
using Ical.Net.DataTypes;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.BusinessLogic.Models;
using TaskManagementSystem.Shared.Helpers;
using Calendar=Ical.Net.Calendar;

namespace TaskManagementSystem.BusinessLogic.Services.Implementations;

public class CalendarService
{
    private readonly List<CalendarEvent> events = new()
    {
        new CalendarEvent(Guid.NewGuid(), "Очень важное событие", null, DateTime.UtcNow, DateTime.UtcNow.AddDays(-3).AddHours(4), DateTime.UtcNow),
        new CalendarEvent(Guid.NewGuid(), "Сделать проект", null, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(100), DateTime.UtcNow),
        new CalendarEvent(Guid.NewGuid(), "Сделать задание", null, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), DateTime.UtcNow)
    };

    private readonly ILogger<CalendarService> logger;

    private readonly List<CalendarEvent> recurrentEvents = new()
    {
        new CalendarEvent(Guid.NewGuid(), "Почистить зубы 1", null,
        new DateTime(2000, 1, 1, 8, 00, 00, DateTimeKind.Utc),
        new DateTime(2000, 1, 1, 8, 10, 00, new JulianCalendar()),
        DateTime.UtcNow,
        new RecurrentEventSettings(date => date.AddYears(1), DateTime.Now)),

        new CalendarEvent(Guid.NewGuid(), "День Рождения меня", null,
        new DateTime(2000, 6, 22, 0, 00, 00, DateTimeKind.Utc),
        new DateTime(2000, 6, 22, 23, 59, 59, DateTimeKind.Utc),
        DateTime.UtcNow,
        new RecurrentEventSettings(date => date.AddYears(1))),
        
        new CalendarEvent(Guid.NewGuid(), "День знаний", null,
        new DateTime(2007, 9, 1, 0, 00, 00, DateTimeKind.Utc),
        new DateTime(2007, 6, 1, 23, 59, 59, DateTimeKind.Utc),
        DateTime.UtcNow,
        new RecurrentEventSettings(date => date.AddYears(1), 11))
    };

    public CalendarService(ILogger<CalendarService> logger)
    {
        this.logger = logger;
    }
    
    public async Task<Result<IEnumerable<CalendarEvent>>> GetEvents2(DateTime utcStartTime, DateTime utcEndTime)
    {
        if (utcStartTime - utcEndTime >= TimeSpan.FromDays(55))
        {
            return Result<IEnumerable<CalendarEvent>>.Error("55 days limit range");
        }

        if (utcStartTime.Kind != DateTimeKind.Utc)
        {
            return Result<IEnumerable<CalendarEvent>>.Error("DateTime Kind is not UTC");
        }

        if (utcEndTime.Kind != DateTimeKind.Utc)
        {
            return Result<IEnumerable<CalendarEvent>>.Error("DateTime Kind is not UTC");
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        var result = events.AsParallel().Where(ev => DateRangeHelper.IsEventInCalendarRange(ev.UtcStartTime, ev.UtcEndTime, utcStartTime, utcEndTime)).ToList();
        
        var result2 = result.Union(recurrentEvents.AsParallel().SelectMany(x => CalculateRecurrentEvent(x, utcStartTime, utcEndTime)).ToList());

        stopwatch.Stop();
        logger.Log(LogLevel.Critical, $"My:{stopwatch.Elapsed:g}");
        
        return Result<IEnumerable<CalendarEvent>>.Success(result2);
    }

    public async Task<Result<IEnumerable<CalendarEvent>>> GetEvents(DateTime utcStartTime, DateTime utcEndTime)
    {
        if (utcStartTime - utcEndTime >= TimeSpan.FromDays(55))
        {
            return Result<IEnumerable<CalendarEvent>>.Error("55 days limit range");
        }

        if (utcStartTime.Kind != DateTimeKind.Utc)
        {
            return Result<IEnumerable<CalendarEvent>>.Error("DateTime Kind is not UTC");
        }

        if (utcEndTime.Kind != DateTimeKind.Utc)
        {
            return Result<IEnumerable<CalendarEvent>>.Error("DateTime Kind is not UTC");
        }

        var result = recurrentEvents.AsParallel().Select(x => new Ical.Net.CalendarComponents.CalendarEvent
        {
            Name = x.Name,
            Start = new CalDateTime(x.UtcStartTime),
            End = new CalDateTime(x.UtcEndTime),
            RecurrenceRules = new List<RecurrencePattern>
            {
                new(FrequencyType.Yearly)
            }
        });

        Calendar calendar = new();
        
        calendar.Events.AddRange(result);

        Stopwatch stopwatch = Stopwatch.StartNew();
        var converted = calendar.GetOccurrences(utcStartTime, utcEndTime).AsParallel().Select(x=> new CalendarEvent(Guid.Empty, ((Ical.Net.CalendarComponents.CalendarEvent)x.Source).Name, null, x.Period.StartTime.AsUtc, x.Period.EndTime.AsUtc, DateTime.MinValue)).ToList();
        stopwatch.Stop();
        
        logger.Log(LogLevel.Critical, $"ICalNet:{stopwatch.Elapsed:g}");
       
        return Result<IEnumerable<CalendarEvent>>.Success(Enumerable.Empty<CalendarEvent>());
    }
    
    private IEnumerable<CalendarEvent> CalculateRecurrentEvent(CalendarEvent recurrentEvent, DateTime startTime, DateTime endTime)
    {
        List<CalendarEvent> result = new();
        
        var periodFunc = recurrentEvent.RecurrentSettings.PeriodFunc;
        int maxEventCount = recurrentEvent.RecurrentSettings.EventCount ?? int.MaxValue;
        DateTime until = recurrentEvent.RecurrentSettings.Until ?? DateTime.MaxValue;
        
        DateTime eventStartTime = recurrentEvent.UtcStartTime;
        DateTime eventEndTime = recurrentEvent.UtcEndTime;
        int eventCount = 1;

        if (startTime >= until)
        {
            return result;
        }

        while (eventStartTime <= endTime && eventCount <= maxEventCount && eventStartTime <= until)
        {
            if (DateRangeHelper.IsEventInCalendarRange(eventStartTime, eventEndTime, startTime, endTime))
            {
                result.Add(recurrentEvent with { UtcStartTime = eventStartTime, UtcEndTime = eventEndTime });
            }

            eventStartTime = periodFunc(eventStartTime);
            eventEndTime = periodFunc(eventEndTime);
            eventCount++;
        }

        return result;
    }
}