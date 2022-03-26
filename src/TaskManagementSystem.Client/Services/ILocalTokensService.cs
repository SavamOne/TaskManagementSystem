namespace TaskManagementSystem.Client.Services;

public interface ILocalTokensService
{
	Task<string?> GetAccessTokenAsync();

	Task<string?> GetRefreshTokenAsync();

	Task SetAccessAndRefreshTokenAsync(string accessToken, string refreshToken);

	Task ClearTokens();
}