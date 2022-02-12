using TaskManagementSystem.Client.Helpers;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Client.Services.Implementations;

public class LocalStorageService : ILocalStorageService
{
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";

    private readonly ILocalStorageWrapper wrapper;

    public LocalStorageService(ILocalStorageWrapper wrapper)
    {
        this.wrapper = wrapper.AssertNotNull();
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        return await wrapper.GetStringAsync(AccessTokenKey);
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        return await wrapper.GetStringAsync(RefreshTokenKey);
    }

    public async Task SetAccessAndRefreshTokenAsync(string accessToken, string refreshToken)
    {
        await wrapper.SetStringAsync(AccessTokenKey, accessToken);
        await wrapper.SetStringAsync(RefreshTokenKey, refreshToken);
    }

    public async Task ClearTokens()
    {
        await wrapper.RemoveAsync(AccessTokenKey);
        await wrapper.RemoveAsync(RefreshTokenKey);
    }
}