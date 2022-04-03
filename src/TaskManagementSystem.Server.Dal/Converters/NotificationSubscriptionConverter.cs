using TaskManagementSystem.Server.Dal.DalModels;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Dal.Converters;

public static class NotificationSubscriptionConverter
{
	public static DalNotificationSubscription ToDalSubscription(this NotificationSubscription notificationSubscription)
	{
		return new DalNotificationSubscription
		{
			UserId = notificationSubscription.UserId,
			Auth = notificationSubscription.Auth,
			P256dh = notificationSubscription.P256dh,
			Url = notificationSubscription.Url
		};
	}

	public static NotificationSubscription ToSubscription(this DalNotificationSubscription notificationSubscription)
	{
		return new NotificationSubscription(notificationSubscription.UserId,
			notificationSubscription.Url,
			notificationSubscription.P256dh,
			notificationSubscription.Auth);
	}
}