using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.BusinessLogic.Dal.Repositories;
using TaskManagementSystem.BusinessLogic.Extensions;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Dal.Extensions;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Services.Implementations;

public class CalendarEventNotificationService
{
	private readonly ConcurrentDictionary<(Guid, Guid), CalendarEventNotification> eventNotifications = new ();
	
	private readonly ICalendarEventParticipantRepository eventParticipantRepository;
	private readonly ICalendarEventRepository eventRepository;
	private readonly ILogger<CalendarEventNotificationService> logger;

	public CalendarEventNotificationService(ICalendarEventParticipantRepository eventParticipantRepository,
		ICalendarEventRepository eventRepository,
		ILogger<CalendarEventNotificationService> logger)
	{
		this.eventParticipantRepository = eventParticipantRepository;
		this.eventRepository = eventRepository;
		this.logger = logger;
	}

	public async Task ExecuteAsync(Func<CalendarEventNotification, Task> notificationCallback, CancellationToken token)
	{
		notificationCallback.AssertNotNull();

		await MainProcessingLoop(notificationCallback, token);
	}

	private async Task MainProcessingLoop(Func<CalendarEventNotification, Task> func, CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			DateTime minuteToSend = DateTime.UtcNow.StripSeconds();
			
			foreach (CalendarEventNotification notification in eventNotifications.Values
			   .OrderBy(x=> x.NotificationTimeUtc))
			{
				if(notification.NotificationTimeUtc > minuteToSend)
				{
					break;
				}

				_ = TrySendNotificationAsync(func, notification);
				
				if (stoppingToken.IsCancellationRequested)
				{
					return;
				}
			}
			
			logger.LogDebug("{0:HH:mm:ss:ffff} - Getting events", DateTime.Now);

			DateTime nextMinute = minuteToSend.AddMinutes(1);
			uint count = await UpdateNotificationsAsync(nextMinute);
			
			logger.LogDebug("Found {0} events with start time >= {1:HH:mm}", count, nextMinute.ToLocalTime());

			await DelayUntil(stoppingToken, nextMinute);
			
			logger.LogDebug("{0:HH:mm:ss:ffff} - Next iteration", DateTime.Now);
		}
	}

	private async Task TrySendNotificationAsync(Func<CalendarEventNotification, Task> func, CalendarEventNotification notification)
	{
		try
		{
			await func(notification);
			eventNotifications.TryRemove(notification.Key, out _);
		}
		catch (Exception e)
		{
			logger.LogCritical(e, "Critical error while sending notification");
		}
	}

	private async Task<uint> UpdateNotificationsAsync(DateTime start)
	{
		uint count = 0;
		var events = await eventRepository.GetAllStandardEventsWithStartTimeInRange(start, start + TimeSpan.FromDays(8));

		foreach (CalendarEvent @event in events)
		{
			var participants = await eventParticipantRepository.GetByEventId(@event.Id);

			var notifications = participants.AsParallel()
			   .Where(x => x.State is not EventParticipantState.Rejected && x.IsParticipantOrCreator())
			   .Select(x =>
					new CalendarEventNotification(x.CalendarParticipant!.UserId,
						@event.Id,
						@event.StartTimeUtc - x.NotifyBefore,
						@event.Name));

			foreach (CalendarEventNotification notification in notifications)
			{
				eventNotifications.AddOrUpdate(notification.Key, notification, (_, _) => notification);
				count++;
			}
		}
		
		return count;
	}

	private async Task DelayUntil(CancellationToken stoppingToken, DateTime until)
	{
		DateTime now = DateTime.UtcNow;
		TimeSpan sleepInterval = until - now;
		
		if (sleepInterval >= TimeSpan.Zero)
		{
			await Task.Delay(sleepInterval, stoppingToken);
		}
		else
		{
			logger.LogCritical("Sleep interval is negative {0:g}", sleepInterval);
		}
	}
}