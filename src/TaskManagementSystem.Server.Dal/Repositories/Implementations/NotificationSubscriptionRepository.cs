using Dapper;
using Dommel;
using TaskManagementSystem.Server.Dal.Converters;
using TaskManagementSystem.Server.Dal.DalModels;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Dal.Repositories.Implementations;

public class NotificationSubscriptionRepository : Repository<DalNotificationSubscription>, INotificationSubscriptionRepository
{
	public NotificationSubscriptionRepository(DatabaseConnectionProvider connectionProvider)
		: base(connectionProvider) {}

	public async Task InsertOrReplaceAsync(NotificationSubscription subscription)
	{
		const string insertOrUpdateSql = "INSERT INTO Notification_Subscription (User_Id, Url, Auth, P256dh) "
									   + "VALUES (@Userid, @Url, @Auth, @P256dh) "
									   + "ON CONFLICT (Url) DO UPDATE SET User_Id = @Userid, Auth = @Auth, P256dh = @P256dh";
		
		await GetConnection().ExecuteAsync(insertOrUpdateSql, subscription.ToDalSubscription());
	}
	
	public async Task DeleteAsync(string url)
	{
		await DeleteMultipleAsync(x => x.Url == url);
	}

	public async Task<ICollection<NotificationSubscription>> GetForUserId(Guid userId)
	{
		var dalSubscriptions = await SelectAsync(x => x.UserId == userId);

		return dalSubscriptions.Select(x => x.ToSubscription()).ToList();
	}
}