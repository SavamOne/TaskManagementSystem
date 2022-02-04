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

        ProcessRefreshTokensResponse(result);

        return result;
    }

    public async Task<LoginResponse> LoginUserAsync(LoginRequest request)
    {
        LoginResponse result =
            await SendAnonymousRequestAsync<LoginRequest, LoginResponse>("Api/V1/User/Login", HttpMethod.Post, request);

        ProcessRefreshTokensResponse(result);

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

        ProcessRefreshTokensResponse(result);
    }

    private void ProcessRefreshTokensResponse(RefreshTokensResponse response)
    {
        if (response.IsSuccess)
        {
            StorageService.SetAccessAndRefreshTokenAsync(response.Tokens!.AccessToken, response.Tokens!.RefreshToken);
            stateProvider.ChangeAuthenticationState(true);
            return;
        }

        StorageService.ClearTokens();
        navigationManager.NavigateTo("Login");
        stateProvider.ChangeAuthenticationState(false);
        throw new Exception(response.ErrorDescription);
    }
}