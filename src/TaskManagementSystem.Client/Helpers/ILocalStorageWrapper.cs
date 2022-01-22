namespace TaskManagementSystem.Client.Helpers;

public interface ILocalStorageWrapper
{
    Task SetAsync<T>(string key, T item);

    Task SetStringAsync(string key, string value);

    Task<T> GetAsync<T>(string key);

    Task<string> GetStringAsync(string key);

    Task RemoveAsync(string key);
}