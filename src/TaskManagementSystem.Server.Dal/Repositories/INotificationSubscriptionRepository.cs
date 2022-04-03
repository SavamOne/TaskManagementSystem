using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Dal.Repositories;

public interface INotificationSubscriptionRepository
{
	Task InsertOrReplaceAsync(NotificationSubscription subscription);

	Task DeleteAsync(string url);

	Task<ICollection<NotificationSubscription>> GetForUserId(Guid userId);
}