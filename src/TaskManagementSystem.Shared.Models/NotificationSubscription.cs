using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public class NotificationSubscription
{
	public NotificationSubscription(Guid userId,
		string url,
		string p256dh,
		string auth)
	{
		UserId = userId;
		Url = url.AssertNotNullOrWhiteSpace();
		P256dh = p256dh.AssertNotNull();
		Auth = auth.AssertNotNull();
	}

	public Guid UserId { get; }

	public string Url { get; }

	public string P256dh { get; }

	public string Auth { get; }
}