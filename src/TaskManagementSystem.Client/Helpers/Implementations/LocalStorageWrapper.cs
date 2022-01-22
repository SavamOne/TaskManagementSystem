using System.Text.Json;
using Microsoft.JSInterop;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Client.Helpers.Implementations;

public class LocalStorageWrapper : ILocalStorageWrapper
{
    private readonly IJSRuntime jsRuntime;

    public LocalStorageWrapper(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async Task SetStringAsync(string key, string value)
    {
        key.AssertNotNullOrWhiteSpace(nameof(key));
        value.AssertNotNullOrWhiteSpace(nameof(value));
        
        await jsRuntime.InvokeVoidAsync("set", key, value);
    }
    
    public async Task SetAsync<T>(string key, T item)
    {
        item.AssertNotNull(nameof(item));
        
        string data = JsonSerializer.Serialize(item);
        await SetStringAsync(key, data);
    }

    public async Task<string?> GetStringAsync(string key)
    {
        key.AssertNotNullOrWhiteSpace(nameof(key));
        
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
        key.AssertNotNullOrWhiteSpace(nameof(key));
        
        await jsRuntime.InvokeAsync<string>("remove", key);
    }
}