namespace TaskManagementSystem.Client.Services;

public interface ILocalStorageService
{
    Task<string> GetAccessTokenAsync();

    Task<string> GetRefreshTokenAsync();

    Task SetAccessAndRefreshTokenAsync(string accessToken, string refreshToken);

    Task ClearTokens();
}