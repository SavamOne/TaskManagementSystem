namespace TaskManagementSystem.Client.Helpers;

public interface IJSInteropWrapper
{
	Task SetAsync<T>(string key, T item);

	Task SetStringAsync(string key, string value);

	Task<T?> GetAsync<T>(string key);

	Task<string?> GetStringAsync(string key);

	Task RemoveAsync(string key);

	Task<string> GetInnerTextByIdAsync(string id);
}