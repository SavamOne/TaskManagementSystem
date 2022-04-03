using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Services;

public interface INotificationService
{
	Task AddSubscriptionAsync(NotificationSubscription subscription);

	Task DeleteSubscriptionAsync(string url);

	Task SendNotificationAsync(Guid userId, WebPushPayload message);

	Task SendNotificationAsync(ISet<Guid> userIds, WebPushPayload message);

	string GetPublicKey();
}