namespace TaskManagementSystem.Server.Workers;

public interface IScopedHostedService
{
	Task ExecuteAsync(CancellationToken stoppingToken);
}