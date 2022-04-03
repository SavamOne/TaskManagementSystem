namespace TaskManagementSystem.Server.Workers.Implementations;

public sealed class ScopedBackgroundServiceProvider : BackgroundService
{
	private readonly ILogger<ScopedBackgroundServiceProvider> logger;
	private readonly IServiceProvider serviceProvider;

	public ScopedBackgroundServiceProvider(IServiceProvider serviceProvider, ILogger<ScopedBackgroundServiceProvider> logger)
	{
		this.serviceProvider = serviceProvider;
		this.logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using (IServiceScope scope = serviceProvider.CreateScope())
		{
			var tasks = scope.ServiceProvider
			   .GetServices<IScopedHostedService>()
			   .Select(x => CreateWaitingTask(x, stoppingToken))
			   .ToArray();

			await Task.WhenAll(tasks);
		}

		logger.LogInformation($"{nameof(ScopedBackgroundServiceProvider)} is stopped.");
	}

	private async Task CreateWaitingTask(IScopedHostedService service, CancellationToken stoppingToken)
	{
		string serviceName = service.GetType().Name;
		try
		{
			await service.ExecuteAsync(stoppingToken);
		}
		catch (OperationCanceledException)
		{
			logger.LogWarning("{0} is cancelled.", serviceName);
		}
		catch (Exception e)
		{
			logger.LogCritical(e, "Critical error in {0}.", serviceName);
		}

		logger.LogInformation("{0} is stopped.", serviceName);
	}
}