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
        return await SendRequestAsync<WeatherForecast[]>("WeatherForecast", HttpMethod.Get);
    }

    public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest request)
    {
        RegisterResponse result =
            await SendAnonymousRequestAsync<RegisterRequest, RegisterResponse>("Api/V1/User/Register", HttpMethod.Post,
            request);

        await ProcessRefreshTokensResponse(result);

        return result;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        LoginResponse result =
            await SendAnonymousRequestAsync<LoginRequest, LoginResponse>("Api/V1/User/Login", HttpMethod.Post, request);

        await ProcessRefreshTokensResponse(result);

        return result;
    }

    public async Task LogoutAsync()
    {
        await StorageService.ClearTokens();
        navigationManager.NavigateTo("Login");
        stateProvider.ChangeAuthenticationState(false);
    }
    
    public async Task<GetUserInfoResponse> GetUserInfoAsync()
    {
        GetUserInfoResponse result =
            await SendRequestAsync<GetUserInfoResponse>("Api/V1/User/GetInfo", HttpMethod.Post);

        return result;
    }
    
    public async Task<ChangePasswordResponse> ChangeUserPasswordAsync(ChangePasswordRequest request)
    {
        ChangePasswordResponse result =
            await SendRequestAsync<ChangePasswordRequest, ChangePasswordResponse>("Api/V1/User/ChangePassword", HttpMethod.Post, request);

        return result;
    }
    
    public async Task<ChangeUserInfoResponse> ChangeUserInfoAsync(ChangeUserInfoRequest request)
    {
        ChangeUserInfoResponse result =
            await SendRequestAsync<ChangeUserInfoRequest, ChangeUserInfoResponse>("Api/V1/User/ChangeInfo", HttpMethod.Post, request);

        return result;
    }

    public async Task<CalendarResponse> GetEventsForMonth(CalendarGetEventsRequest request)
    {
        CalendarResponse result =
            await SendRequestAsync<CalendarGetEventsRequest, CalendarResponse>("Api/V1/CalendarEvents/GetEvents", HttpMethod.Post, request);

        return result;
    }

    protected override async Task RefreshTokens()
    {
        string refreshToken = await StorageService.GetRefreshTokenAsync();
        RefreshTokensRequest request = new(refreshToken);

        RefreshTokensResponse result =
            await SendAnonymousRequestAsync<RefreshTokensRequest, RefreshTokensResponse>(
            "Api/V1/User/Refresh", HttpMethod.Post, request);

        await ProcessRefreshTokensResponse(result);
    }

    private async Task ProcessRefreshTokensResponse(RefreshTokensResponse response)
    {
        if (response.IsSuccess)
        {
            await StorageService.SetAccessAndRefreshTokenAsync(response.Tokens!.AccessToken, response.Tokens!.RefreshToken);
            stateProvider.ChangeAuthenticationState(true);
            return;
        }

        await LogoutAsync();
    }
}