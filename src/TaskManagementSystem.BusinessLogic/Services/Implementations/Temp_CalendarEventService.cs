using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.BusinessLogic.Models.Exceptions;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Resources;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Services.Implementations;

public class Temp_CalendarEventService
{

	private readonly List<Temp_CalendarEvent> events = new()
	{
		new Temp_CalendarEvent(Guid.NewGuid(),
			"Very important event",
			null,
			DateTime.UtcNow,
			DateTime.UtcNow.AddDays(-3).AddHours(4),
			DateTime.UtcNow),

		new Temp_CalendarEvent(Guid.NewGuid(),
			"Making this project",
			null,
			new DateTime(2022,
				1,
				3,
				15,
				00,
				00,
				DateTimeKind.Utc),
			DateTime.UtcNow.AddDays(100),
			DateTime.UtcNow),

		new Temp_CalendarEvent(Guid.NewGuid(),
			"Wake up",
			null,
			new DateTime(2000,
				1,
				2,
				8,
				00,
				00,
				DateTimeKind.Utc),
			new DateTime(2000,
				1,
				2,
				8,
				10,
				00,
				DateTimeKind.Utc),
			DateTime.UtcNow,
			new Temp_RecurrentEventSettings(date => date.AddDays(7), DateTime.UtcNow)),

		new Temp_CalendarEvent(Guid.NewGuid(),
			"My Birthday",
			null,
			new DateTime(2000,
				6,
				22,
				0,
				00,
				00,
				DateTimeKind.Utc),
			new DateTime(2000,
				6,
				22,
				23,
				59,
				59,
				DateTimeKind.Utc),
			DateTime.UtcNow,
			new Temp_RecurrentEventSettings(date => date.AddYears(1))),

		new Temp_CalendarEvent(Guid.NewGuid(),
			"Knowledge day",
			null,
			new DateTime(2007,
				9,
				1,
				0,
				00,
				00,
				DateTimeKind.Utc),
			new DateTime(2007,
				6,
				1,
				23,
				59,
				59,
				DateTimeKind.Utc),
			DateTime.UtcNow,
			new Temp_RecurrentEventSettings(date => date.AddYears(1), 11))
	};

	private readonly ILogger<Temp_CalendarEventService> logger;

	public Temp_CalendarEventService(ILogger<Temp_CalendarEventService> logger)
	{
		this.logger = logger;
	}

	public async Task<IEnumerable<Temp_CalendarEvent>> GetEvents(DateTime utcStartTime, DateTime utcEndTime)
	{
		if (utcStartTime - utcEndTime >= TimeSpan.FromDays(60))
		{
			throw new BusinessLogicException(LocalizedResources.DaysLimitOutOfRange, 60);
		}

		if (utcStartTime.Kind != DateTimeKind.Utc || utcEndTime.Kind != DateTimeKind.Utc)
		{
			throw new BusinessLogicException(LocalizedResources.DateIsNotInUtc);
		}

		Stopwatch stopwatch = Stopwatch.StartNew();

		var suitableRegularEvents = events.AsParallel()
		   .Where(ev => ev.RecurrentSettings == null && DateRangeHelper.IsEventInCalendarRange(ev.UtcStartTime, ev.UtcEndTime, utcStartTime, utcEndTime));
		var suitableRecurrentEvents = events.AsParallel()
		   .Where(ev => ev.RecurrentSettings != null)
		   .SelectMany(x => CalculateRecurrentEvent(x, utcStartTime, utcEndTime));
		var suitableEvents = suitableRegularEvents.Union(suitableRecurrentEvents).ToList();

		stopwatch.Stop();

		logger.Log(LogLevel.Information, "Get events time:{Elapsed:g}", stopwatch.Elapsed);

		return suitableEvents;
	}

	private IEnumerable<Temp_CalendarEvent> CalculateRecurrentEvent(Temp_CalendarEvent recurrentEventTemp, DateTime calendarStartTime, DateTime calendarEndTime)
	{
		if (recurrentEventTemp.RecurrentSettings == null)
		{
			yield break;
		}

		var periodFunc = recurrentEventTemp.RecurrentSettings.PeriodFunc;
		int maxEventCount = recurrentEventTemp.RecurrentSettings.EventCount ?? int.MaxValue;
		DateTime until = recurrentEventTemp.RecurrentSettings.Until ?? DateTime.MaxValue;

		DateTime eventStartTime = recurrentEventTemp.UtcStartTime;
		DateTime eventEndTime = recurrentEventTemp.UtcEndTime;
		int eventCount = 1;

		if (calendarStartTime >= until)
		{
			yield break;
		}

		while (eventStartTime <= calendarEndTime && eventCount <= maxEventCount && eventStartTime <= until)
		{
			if (DateRangeHelper.IsEventInCalendarRange(eventStartTime, eventEndTime, calendarStartTime, calendarEndTime))
			{
				yield return recurrentEventTemp with
				{
					UtcStartTime = eventStartTime,
					UtcEndTime = eventEndTime
				};
			}

			eventStartTime = periodFunc(eventStartTime);
			eventEndTime = periodFunc(eventEndTime);
			eventCount++;
		}
	}
}