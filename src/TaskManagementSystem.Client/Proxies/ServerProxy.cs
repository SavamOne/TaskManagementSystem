using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Providers;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Contracts;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Proxies;

public class ServerProxy : BaseProxy
{
    private readonly NavigationManager navigationManager;
    private readonly JwtAuthenticationStateProvider stateProvider;

    public ServerProxy(
        HttpClient httpClient,
        ILocalStorageService storageService,
        NavigationManager navigationManager,
        JwtAuthenticationStateProvider stateProvider)
        : base(httpClient, storageService)
    {
        this.navigationManager = navigationManager.AssertNotNull();
        this.stateProvider = stateProvider.AssertNotNull();
    }

    public async Task<WeatherForecast[]> Get()
    {
        return ( await SendRequestAsync<WeatherForecast[]>("WeatherForecast", HttpMethod.Get) ).Value!;
    }

    public async Task<Result<Tokens>> RegisterUserAsync(RegisterRequest request)
    {
        var result = await SendAnonymousRequestAsync<RegisterRequest, Tokens>("Api/V1/User/Register", HttpMethod.Post,
        request);

        await ProcessRefreshTokensResponse(result);

        return result;
    }

    public async Task<Result<Tokens>> LoginAsync(LoginRequest request)
    {
        var result =
            await SendAnonymousRequestAsync<LoginRequest, Tokens>("Api/V1/User/Login", HttpMethod.Post, request);

        await ProcessRefreshTokensResponse(result);

        return result;
    }

    public async Task LogoutAsync()
    {
        await StorageService.ClearTokens();
        navigationManager.NavigateTo("Login");
        stateProvider.ChangeAuthenticationState(false);
    }

    public async Task<Result<UserInfo>> GetUserInfoAsync()
    {
        var result =
            await SendRequestAsync<UserInfo>("Api/V1/User/GetInfo", HttpMethod.Post);

        return result;
    }

    public async Task<Result<UserInfo>> ChangeUserPasswordAsync(ChangePasswordRequest request)
    {
        var result =
            await SendRequestAsync<ChangePasswordRequest, UserInfo>("Api/V1/User/ChangePassword", HttpMethod.Post, request);

        return result;
    }

    public async Task<Result<UserInfo>> ChangeUserInfoAsync(ChangeUserInfoRequest request)
    {
        var result =
            await SendRequestAsync<ChangeUserInfoRequest, UserInfo>("Api/V1/User/ChangeInfo", HttpMethod.Post, request);

        return result;
    }

    public async Task<Result<CalendarEventInfo[]>> GetEventsForMonth(CalendarGetEventsRequest request)
    {
        var result =
            await SendRequestAsync<CalendarGetEventsRequest, CalendarEventInfo[]>("Api/V1/CalendarEvents/GetEvents", HttpMethod.Post, request);

        return result;
    }

    protected override async Task RefreshTokens()
    {
        string refreshToken = await StorageService.GetRefreshTokenAsync();
        RefreshTokensRequest request = new(refreshToken);

        var result =
            await SendAnonymousRequestAsync<RefreshTokensRequest, Tokens>(
            "Api/V1/User/Refresh", HttpMethod.Post, request);

        await ProcessRefreshTokensResponse(result);
    }

    private async Task ProcessRefreshTokensResponse(Result<Tokens> response)
    {
        if (response.IsSuccess)
        {
            await StorageService.SetAccessAndRefreshTokenAsync(response.Value!.AccessToken, response.Value!.RefreshToken);
            stateProvider.ChangeAuthenticationState(true);
            return;
        }

        await LogoutAsync();
    }
}