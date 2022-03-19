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
        ILocalTokensService storageService,
        IToastService toastService,
        NavigationManager navigationManager,
        JwtAuthenticationStateProvider stateProvider,
        ILocalizationService localizationService)
        : base(httpClient, storageService, toastService, localizationService)
    {
        this.navigationManager = navigationManager.AssertNotNull();
        this.stateProvider = stateProvider.AssertNotNull();
    }

    public async Task<WeatherForecast[]> Get()
    {
        return ( await SendRequestAsync<WeatherForecast[]>(new Uri("WeatherForecast", UriKind.Relative), HttpMethod.Get) ).Value!;
    }

    public async Task<Result<Tokens>> RegisterUserAsync(RegisterRequest request)
    {
        var result = await SendAnonymousRequestAsync<RegisterRequest, Tokens>(new Uri("Api/V1/User/Register", UriKind.Relative), HttpMethod.Post,
        request);

        await ProcessRefreshTokensResponse(result);

        return result;
    }

    public async Task<Result<Tokens>> LoginAsync(LoginRequest request)
    {
        var result =
            await SendAnonymousRequestAsync<LoginRequest, Tokens>(new Uri("Api/V1/User/Login", UriKind.Relative), HttpMethod.Post, request);

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
            await SendRequestAsync<UserInfo>(new Uri("Api/V1/User/GetInfo", UriKind.Relative), HttpMethod.Post);

        return result;
    }

    public async Task<Result<UserInfo>> GetUserInfoAsync(GetUserInfoByIdRequest byIdRequest)
    {
        var result =
            await SendRequestAsync<GetUserInfoByIdRequest, UserInfo>(new Uri("Api/V1/User/GetInfoById", UriKind.Relative), HttpMethod.Post, byIdRequest);

        return result;
    }

    public async Task<Result<UserInfo[]>> GetUsersByFilterAsync(GetUserInfosByFilterRequest request)
    {
        var result =
            await SendRequestAsync<GetUserInfosByFilterRequest, UserInfo[]>(new Uri("Api/V1/User/GetInfosByFilter", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }

    public async Task<Result<UserInfo>> ChangeUserPasswordAsync(ChangePasswordRequest request)
    {
        var result =
            await SendRequestAsync<ChangePasswordRequest, UserInfo>(new Uri("Api/V1/User/ChangePassword", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }

    public async Task<Result<UserInfo>> ChangeUserInfoAsync(ChangeUserInfoRequest request)
    {
        var result =
            await SendRequestAsync<ChangeUserInfoRequest, UserInfo>(new Uri("Api/V1/User/ChangeInfo", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }

    public async Task<Result<Temp_CalendarEventInfo[]>> GetEventsForMonth(CalendarGetEventsRequest request)
    {
        var result =
            await SendRequestAsync<CalendarGetEventsRequest, Temp_CalendarEventInfo[]>(new Uri("Api/V1/CalendarEventsController_Temp/GetEvents", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }

    public async Task<Result<IEnumerable<CalendarInfo>>> GetUserCalendars()
    {
        var result =
            await SendRequestAsync<IEnumerable<CalendarInfo>>(new Uri("Api/V1/Calendar/GetMyList", UriKind.Relative), HttpMethod.Post);

        return result;
    }

    public async Task<Result<CalendarInfo>> CreateCalendar(CreateCalendarRequest request)
    {
        var result =
            await SendRequestAsync<CreateCalendarRequest, CalendarInfo>(new Uri("Api/V1/Calendar/Create", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }

    public async Task<Result<CalendarInfo>> EditCalendar(EditCalendarRequest request)
    {
        var result =
            await SendRequestAsync<EditCalendarRequest, CalendarInfo>(new Uri("Api/V1/Calendar/Edit", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }

    public async Task<Result<CalendarWithParticipantUsers>> GetCalendarInfo(GetCalendarInfoRequest request)
    {
        var result =
            await SendRequestAsync<GetCalendarInfoRequest, CalendarWithParticipantUsers>(new Uri("Api/V1/Calendar/GetInfo", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }

    public async Task<Result<CalendarWithParticipantUsers>> AddCalendarParticipants(AddCalendarParticipantsRequest request)
    {
        var result =
            await SendRequestAsync<AddCalendarParticipantsRequest, CalendarWithParticipantUsers>(new Uri("Api/V1/Calendar/AddParticipants", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }

    public async Task<Result<CalendarWithParticipantUsers>> ChangeParticipantsRole(ChangeCalendarParticipantsRoleRequest request)
    {
        var result =
            await SendRequestAsync<ChangeCalendarParticipantsRoleRequest, CalendarWithParticipantUsers>(new Uri("Api/V1/Calendar/ChangeParticipantsRole", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }

    public async Task<Result<CalendarWithParticipantUsers>> DeleteCalendarParticipants(DeleteParticipantsRequest request)
    {
        var result =
            await SendRequestAsync<DeleteParticipantsRequest, CalendarWithParticipantUsers>(new Uri("Api/V1/Calendar/DeleteParticipants", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }

    public async Task<Result<EventInfo>> CreateEvent(CreateEventRequest request)
    {
        var result =
            await SendRequestAsync<CreateEventRequest, EventInfo>(new Uri("Api/V1/CalendarEvent/Create", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }
    
    public async Task<Result<EventInfo>> EditEvent(EditEventRequest request)
    {
        var result =
            await SendRequestAsync<EditEventRequest, EventInfo>(new Uri("Api/V1/CalendarEvent/Edit", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }

    public async Task<Result<EventInfo>> DeleteEvent(DeleteEventRequest request)
    {
        var result =
            await SendRequestAsync<DeleteEventRequest, EventInfo>(new Uri("Api/V1/CalendarEvent/Delete", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }

    public async Task<Result<EventWithParticipants>> AddEventParticipants(AddEventParticipantsRequest request)
    {
        var result =
            await SendRequestAsync<AddEventParticipantsRequest, EventWithParticipants>(new Uri("Api/V1/CalendarEvent/AddParticipants", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }
    
    public async Task<Result<EventWithParticipants>> GetEventInfo(GetEventInfoRequest request)
    {
        var result =
            await SendRequestAsync<GetEventInfoRequest, EventWithParticipants>(new Uri("Api/V1/CalendarEvent/GetInfo", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }
    
    public async Task<Result<IEnumerable<EventInfo>>> GetEventsInPeriod(GetEventsInPeriodRequest request)
    {
        var result =
            await SendRequestAsync<GetEventsInPeriodRequest, IEnumerable<EventInfo>>(new Uri("Api/V1/CalendarEvent/GetInPeriod", UriKind.Relative), HttpMethod.Post, request);

        return result;
    }
    
    protected override async Task RefreshTokens()
    {
        string? refreshToken = await StorageService.GetRefreshTokenAsync();
        RefreshTokensRequest request = new(refreshToken!);

        var result =
            await SendAnonymousRequestAsync<RefreshTokensRequest, Tokens>(
            new Uri("Api/V1/User/Refresh", UriKind.Relative), HttpMethod.Post, request);

        await ProcessRefreshTokensResponse(result);

        if (!result.IsSuccess)
        {
            ToastService.AddSystemErrorToast(result.ErrorDescription!);
        }
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