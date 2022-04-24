using Microsoft.Extensions.Logging;
using TaskManagementSystem.BusinessLogic.Dal.Repositories;
using TaskManagementSystem.BusinessLogic.Extensions;
using TaskManagementSystem.BusinessLogic.Helpers;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Dal.Extensions;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Services.Implementations;

public class CalendarEventNotificationService : ICalendarEventNotificationService
{
	private const int DeltaMinutes = 20;

	private readonly ICalendarEventParticipantRepository eventParticipantRepository;
	private readonly ICalendarEventRepository eventRepository;
	private readonly ILogger<CalendarEventNotificationService> logger;
	private readonly IRecurrentSettingsRepository recurrentSettingsRepository;

	private ICollection<CalendarEventNotification> eventNotifications = Array.Empty<CalendarEventNotification>();

	public CalendarEventNotificationService(ICalendarEventParticipantRepository eventParticipantRepository,
		ICalendarEventRepository eventRepository,
		IRecurrentSettingsRepository recurrentSettingsRepository,
		ILogger<CalendarEventNotificationService> logger)
	{
		this.eventParticipantRepository = eventParticipantRepository;
		this.eventRepository = eventRepository;
		this.recurrentSettingsRepository = recurrentSettingsRepository;
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

			foreach (CalendarEventNotification notification in eventNotifications)
			{
				if (notification.NotificationTimeUtc > minuteToSend)
				{
					break;
				}

				_ = TrySendNotificationAsync(func, notification);

				if (stoppingToken.IsCancellationRequested)
				{
					return;
				}
			}

			logger.LogDebug("Getting events");

			DateTime nextMinute = minuteToSend.AddMinutes(1);
			int count = await UpdateNotificationsAsync(nextMinute);

			logger.LogDebug("Found {0} events with start time >= {1:HH:mm} and <= {2:HH:mm}", count, nextMinute.ToLocalTime(), nextMinute.AddMinutes(DeltaMinutes).ToLocalTime());

			await DelayUntil(stoppingToken, nextMinute);

			logger.LogDebug("Next iteration");
		}
	}

	private async Task TrySendNotificationAsync(Func<CalendarEventNotification, Task> func, CalendarEventNotification notification)
	{
		try
		{
			await func(notification);
		}
		catch (Exception e)
		{
			logger.LogCritical(e, "Critical error while sending notification");
		}
	}

	private async Task<int> UpdateNotificationsAsync(DateTime start)
	{
		var standardEvents = await eventRepository.GetAllStandardEventsWithStartTimeInRange(start, start + TimeSpan.FromDays(8));

		var repeatedEvents = await eventRepository.GetAllRepeatedEvents();
		var recurrentEventSettings = ( await recurrentSettingsRepository.GetForEvents(repeatedEvents.Select(x => x.Id).ToHashSet()) ).ToDictionary(x => x.EventId);

		var orderedEvents = new List<CalendarEventNotification>();

		foreach (CalendarEvent repeatedEvent in repeatedEvents)
		{
			var participants = ( await eventParticipantRepository.GetByEventId(repeatedEvent.Id) )
			   .Where(x => x.State is not CalendarEventParticipantState.Rejected && x.IsParticipantOrCreator())
			   .ToList();

			foreach (CalendarEvent @event in RecurrenceCalculator.Calculate(repeatedEvent, recurrentEventSettings[repeatedEvent.Id], start, start + TimeSpan.FromDays(8)))
			{
				var notifications = participants
				   .Select(x => new CalendarEventNotification(x.CalendarParticipant!.UserId,
						@event.Id,
						@event.RepeatNum,
						@event.StartTimeUtc - x.NotifyBefore,
						@event.Name))
				   .Where(x => x.NotificationTimeUtc >= start && x.NotificationTimeUtc <= start.AddMinutes(DeltaMinutes));

				orderedEvents.AddRange(notifications);
			}

		}

		foreach (CalendarEvent @event in standardEvents)
		{
			var participants = await eventParticipantRepository.GetByEventId(@event.Id);

			var notifications = participants.AsParallel()
			   .Where(x => x.State is not CalendarEventParticipantState.Rejected && x.IsParticipantOrCreator())
			   .Select(x =>
					new CalendarEventNotification(x.CalendarParticipant!.UserId,
						@event.Id,
						@event.RepeatNum,
						@event.StartTimeUtc - x.NotifyBefore,
						@event.Name))
			   .Where(x => x.NotificationTimeUtc >= start && x.NotificationTimeUtc <= start.AddMinutes(DeltaMinutes));

			orderedEvents.AddRange(notifications);
		}

		eventNotifications = orderedEvents.OrderBy(x => x.NotificationTimeUtc).ToList();

		return eventNotifications.Count;
	}

	private async Task DelayUntil(CancellationToken stoppingToken, DateTime until)
	{
		TimeSpan sleepInterval = until - DateTime.UtcNow;
		if (sleepInterval > TimeSpan.Zero)
		{
			await Task.Delay(sleepInterval, stoppingToken);
			while (DateTime.UtcNow < until && !stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(10, stoppingToken);
			}
		}
		else
		{
			logger.LogCritical("Sleep interval is negative {0:g}", DateTime.UtcNow - until);
		}
	}
}