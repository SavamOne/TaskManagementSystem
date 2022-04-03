using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TaskManagementSystem.BusinessLogic.Dal.Repositories;
using TaskManagementSystem.BusinessLogic.Models.Exceptions;
using TaskManagementSystem.Server.Dal.Repositories;
using TaskManagementSystem.Server.Options;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Options;
using WebPush;

namespace TaskManagementSystem.Server.Services.Implementations;

public class NotificationService : INotificationService, IDisposable
{
	private readonly IOptions<WebPushOptions> options;
	private readonly INotificationSubscriptionRepository subscriptionRepository;
	private readonly IUserRepository userRepository;
	private readonly WebPushClient webPushClient;

	public NotificationService(IOptions<WebPushOptions> options, IUserRepository userRepository, INotificationSubscriptionRepository subscriptionRepository)
	{
		this.options = options;
		this.userRepository = userRepository;
		this.subscriptionRepository = subscriptionRepository;
		webPushClient = new WebPushClient();
	}

	public void Dispose()
	{
		webPushClient.Dispose();
	}

	//TODO: AddSubscriptionData
	public async Task AddSubscriptionAsync(NotificationSubscription subscription)
	{
		subscription.AssertNotNull();

		if (await userRepository.GetByIdAsync(subscription.UserId) == null)
		{
			throw new BusinessLogicException("Пользователя с таким идентификатором нет.");
		}

		VapidDetails details = new(options.Value.Subject, options.Value.PublicKey, options.Value.PrivateKey);
		await SendNotificationAsyncImpl(subscription, details, new WebPushPayload("Успешно подписано", ""));
		await subscriptionRepository.InsertOrReplaceAsync(subscription);
	}

	public async Task DeleteSubscriptionAsync(string url)
	{
		url.AssertNotNullOrWhiteSpace();

		await subscriptionRepository.DeleteAsync(url);
	}

	public async Task SendNotificationAsync(Guid userId, WebPushPayload message)
	{
		await SendNotificationAsync(new HashSet<Guid>
			{
				userId
			},
			message);
	}

	public async Task SendNotificationAsync(ISet<Guid> userIds, WebPushPayload message)
	{
		VapidDetails details = new(options.Value.Subject, options.Value.PublicKey, options.Value.PrivateKey);

		foreach (Guid userId in userIds)
		{
			foreach (NotificationSubscription subscription in await subscriptionRepository.GetForUserId(userId))
			{
				await SendNotificationAsyncImpl(subscription, details, message);
			}
		}
	}

	public string GetPublicKey()
	{
		return options.Value.PublicKey!;
	}

	private async Task SendNotificationAsyncImpl(NotificationSubscription subscription, VapidDetails vapidDetails, WebPushPayload payload)
	{
		try
		{
			PushSubscription pushSubscription = new(subscription.Url, subscription.P256dh, subscription.Auth);

			string payloadString = JsonSerializer.Serialize(payload, ApplicationJsonOptions.Options);

			await webPushClient.SendNotificationAsync(pushSubscription, payloadString, vapidDetails);
		}
		catch (WebPushException e)
		{
			await DeleteSubscriptionAsync(subscription.Url);
			if (e.StatusCode == HttpStatusCode.Unauthorized)
			{
				throw new BusinessLogicException("Не удалось подписаться на уведомления. Попробуйте перезайти в систему, если проблема повторяется, смените браузер.");
			}

		}
	}
}