using System.Text.Json;
using Microsoft.JSInterop;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.Helpers.Implementations;

public class JSInteropWrapper : IJSInteropWrapper
{
	private readonly IJSRuntime jsRuntime;

	public JSInteropWrapper(IJSRuntime jsRuntime)
	{
		this.jsRuntime = jsRuntime.AssertNotNull();
	}

	public async Task SetStringAsync(string key, string value)
	{
		key.AssertNotNullOrWhiteSpace();
		value.AssertNotNullOrWhiteSpace();

		await jsRuntime.InvokeVoidAsync("set", key, value);
	}

	public async Task SetAsync<T>(string key, T item)
	{
		item.AssertNotNull();

		string data = JsonSerializer.Serialize(item);
		await SetStringAsync(key, data);
	}

	public async Task<string?> GetStringAsync(string key)
	{
		key.AssertNotNullOrWhiteSpace();

		return await jsRuntime.InvokeAsync<string>("get", key);
	}

	public async Task<T?> GetAsync<T>(string key)
	{
		string? data = await GetStringAsync(key);

		if (string.IsNullOrEmpty(data))
		{
			return default;
		}

		return JsonSerializer.Deserialize<T>(data)!;
	}

	public async Task RemoveAsync(string key)
	{
		key.AssertNotNullOrWhiteSpace();

		await jsRuntime.InvokeAsync<string>("remove", key);
	}
	public async Task<string> GetInnerTextByIdAsync(string id)
	{
		return await jsRuntime.InvokeAsync<string>("getInnerTextById", id);
	}

	public async Task<AddNotificationSubscribeRequest?> TryRequestNotificationSubscriptionAsync(string publicKey)
	{
		return await jsRuntime.InvokeAsync<AddNotificationSubscribeRequest?>("requestNotificationSubscription", publicKey);
	}

	public async Task<string?> UnsubscribeFromNotificationsAsync()
	{
		return await jsRuntime.InvokeAsync<string>("unsubscribeFromNotifications");
	}

	public async Task<bool> NeedToSubscribeAsync()
	{
		return await jsRuntime.InvokeAsync<bool>("needToSubscribe");
	}
}