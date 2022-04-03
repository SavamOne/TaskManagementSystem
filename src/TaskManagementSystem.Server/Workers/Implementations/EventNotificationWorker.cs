using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Services.Implementations;
using TaskManagementSystem.Server.Services;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Workers.Implementations;

public class EventNotificationWorker : IScopedHostedService
{
	private readonly CalendarEventNotificationService eventNotificationService;
	private readonly ILogger<EventNotificationWorker> logger;
	private readonly INotificationService notificationService;
	private readonly IServiceProvider serviceProvider;

	public EventNotificationWorker(CalendarEventNotificationService eventNotificationService,
		INotificationService notificationService,
		ILogger<EventNotificationWorker> logger,
		IServiceProvider serviceProvider)
	{
		this.eventNotificationService = eventNotificationService;
		this.notificationService = notificationService;
		this.logger = logger;
		this.serviceProvider = serviceProvider;
	}

	public async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await eventNotificationService.ExecuteAsync(NotifyCallback, stoppingToken);

		logger.LogDebug("Shutdown");
	}

	private async Task NotifyCallback(CalendarEventNotification notification)
	{
		using IServiceScope scope = serviceProvider.CreateScope();
		INotificationService localNotificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

		await localNotificationService.SendNotificationAsync(notification.UserId, new WebPushPayload(notification.EventName, "/"));
	}
}