using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;
using TaskManagementSystem.BusinessLogic.Services;
using TaskManagementSystem.Server.Services;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Workers.Implementations;

public class EventNotificationWorker : IScopedHostedService
{
	private readonly ISet<Guid> notifiedEvents = new HashSet<Guid>();
	
	private readonly INotificationService notificationService;
	private readonly ICalendarEventService calendarEventService;
	private readonly ILogger<EventNotificationWorker> logger;

	public EventNotificationWorker(INotificationService notificationService, ICalendarEventService calendarEventService, ILogger<EventNotificationWorker> logger)
	{
		this.notificationService = notificationService;
		this.calendarEventService = calendarEventService;
		this.logger = logger;
	}

	public async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		Guid myUserId = Guid.Parse("7408793c-134d-4b1b-8aff-2159d0685af1");
		Guid myCalendarId = Guid.Parse("c693262c-b424-4f46-a911-a0bec25ac2f5");
		
		await notificationService.SendNotificationAsync(myUserId, new WebPushPayload("Сервер запущен!!!!!", String.Empty));

		while (!stoppingToken.IsCancellationRequested)
		{
			DateTimeOffset current = DateTimeOffset.UtcNow;
			DateTimeOffset delta = current + TimeSpan.FromMinutes(20);
			var events = await calendarEventService.GetEventsInPeriodAsync(new GetEventsInPeriodData(myUserId,
				myCalendarId,
				current,
				delta));

			logger.LogDebug("Found {0}", events.Count);

			var toNotify = events.Where(x => !notifiedEvents.Contains(x.Id) && x.StartTimeUtc - TimeSpan.FromMinutes(10) <= DateTime.UtcNow);
			foreach (CalendarEvent calendarEvent in toNotify)
			{
				await notificationService.SendNotificationAsync(myUserId, new WebPushPayload(calendarEvent.Name, "/"));
				notifiedEvents.Add(calendarEvent.Id);

				logger.LogDebug("NOTIFIED!!!!");
			}

			logger.LogDebug("Sleep");
			await Task.Delay(20000, stoppingToken);
			logger.LogDebug("Wake Up");
		}
		
		logger.LogDebug("Shutdown");
	}
}