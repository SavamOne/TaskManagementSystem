using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.Helpers;

public interface IJSInteropWrapper
{
	Task SetAsync<T>(string key, T item);

	Task SetStringAsync(string key, string value);

	Task<T?> GetAsync<T>(string key);

	Task<string?> GetStringAsync(string key);

	Task RemoveAsync(string key);

	Task<string> GetInnerTextByIdAsync(string id);

	Task<string> GetValueByIdAsync(string id);

	Task<AddNotificationSubscribeRequest?> TryRequestNotificationSubscriptionAsync(string publicKey);

	Task<string?> UnsubscribeFromNotificationsAsync();

	Task<bool> NeedToSubscribeAsync();
}