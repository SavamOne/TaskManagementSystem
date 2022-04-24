using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Helpers;
using TaskManagementSystem.Client.Providers;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.Proxies;

public class ServerProxy : BaseProxy
{
	private readonly IJSInteropWrapper interopWrapper;
	private readonly NavigationManager navigationManager;
	private readonly JwtAuthenticationStateProvider stateProvider;

	public ServerProxy(HttpClient httpClient,
		ILocalTokensService storageService,
		IToastService toastService,
		NavigationManager navigationManager,
		JwtAuthenticationStateProvider stateProvider,
		ILocalizationService localizationService,
		IJSInteropWrapper interopWrapper)
		: base(httpClient, storageService, toastService, localizationService)
	{
		this.interopWrapper = interopWrapper;
		this.navigationManager = navigationManager.AssertNotNull();
		this.stateProvider = stateProvider.AssertNotNull();
	}

	public async Task<Result<Tokens>> RegisterUserAsync(RegisterRequest request)
	{
		var result = await SendAnonymousRequestAsync<RegisterRequest, Tokens>(new Uri("Api/V1/User/Register", UriKind.Relative),
			HttpMethod.Post,
			request);

		if (result.IsSuccess)
		{
			await ProcessRefreshTokensResponse(result);
		}

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
		string? subscriptionEndpoint = await interopWrapper.UnsubscribeFromNotificationsAsync();
		if (subscriptionEndpoint != null)
		{
			await UnsubscribeFromNotifications(new DeleteNotificationSubscribeRequest(subscriptionEndpoint));
		}

		await StorageService.ClearTokens();
		stateProvider.ChangeAuthenticationState(false);
		navigationManager.NavigateTo("Login");
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
			await SendRequestAsync<GetUserInfosByFilterRequest, UserInfo[]>(new Uri("Api/V1/User/GetByFilter", UriKind.Relative), HttpMethod.Post, request);

		return result;
	}

	public async Task<Result<UserInfo>> ChangeUserPasswordAsync(ChangePasswordRequest request)
	{
		var result =
			await SendRequestAsync<ChangePasswordRequest, UserInfo>(new Uri("Api/V1/User/ChangePassword", UriKind.Relative), HttpMethod.Post, request);

		return result;
	}

	public async Task<Result<UserInfo>> EditUserInfoAsync(EditUserInfoRequest request)
	{
		var result =
			await SendRequestAsync<EditUserInfoRequest, UserInfo>(new Uri("Api/V1/User/EditInfo", UriKind.Relative), HttpMethod.Post, request);

		return result;
	}

	public async Task<Result<IEnumerable<CalendarInfo>>> GetUserCalendars()
	{
		var result =
			await SendRequestAsync<IEnumerable<CalendarInfo>>(new Uri("Api/V1/Calendar/GetMyCalendars", UriKind.Relative), HttpMethod.Post);

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

	public async Task<Result<CalendarWithParticipantUsers>> EditCalendarParticipants(EditCalendarParticipantsRequest request)
	{
		var result =
			await SendRequestAsync<EditCalendarParticipantsRequest, CalendarWithParticipantUsers>(new Uri("Api/V1/Calendar/EditParticipants", UriKind.Relative), HttpMethod.Post, request);

		return result;
	}

	public async Task<Result<ICollection<CalendarParticipantUser>>> GetCalendarParticipantsByFilter(GetCalendarParticipantsByFilterRequest request)
	{
		var result =
			await SendRequestAsync<GetCalendarParticipantsByFilterRequest, ICollection<CalendarParticipantUser>>(new Uri("Api/V1/Calendar/GetParticipantsByFilter", UriKind.Relative),
				HttpMethod.Post,
				request);

		return result;
	}

	public async Task<Result<ICollection<CalendarNameResponse>>> GetCalendarName(GetCalendarNamesRequest request)
	{
		var result =
			await SendRequestAsync<GetCalendarNamesRequest, ICollection<CalendarNameResponse>>(new Uri("Api/V1/Calendar/GetNames", UriKind.Relative),
				HttpMethod.Post,
				request);

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

	public async Task<Result<bool>> DeleteEvent(DeleteEventRequest request)
	{
		var result =
			await SendRequestAsync<DeleteEventRequest, bool>(new Uri("Api/V1/CalendarEvent/Delete", UriKind.Relative), HttpMethod.Post, request);

		return result;
	}

	public async Task<Result<EventWithParticipants>> AddEventParticipants(AddEventParticipantsRequest request)
	{
		var result =
			await SendRequestAsync<AddEventParticipantsRequest, EventWithParticipants>(new Uri("Api/V1/CalendarEvent/AddParticipants", UriKind.Relative), HttpMethod.Post, request);

		return result;
	}

	public async Task<Result<EventWithParticipants>> EditEventParticipants(EditEventParticipantsRequest request)
	{
		var result =
			await SendRequestAsync<EditEventParticipantsRequest, EventWithParticipants>(new Uri("Api/V1/CalendarEvent/EditParticipants", UriKind.Relative), HttpMethod.Post, request);

		return result;
	}

	public async Task<Result<EventWithParticipants>> GetEventInfo(GetEventInfoRequest request)
	{
		var result =
			await SendRequestAsync<GetEventInfoRequest, EventWithParticipants>(new Uri("Api/V1/CalendarEvent/GetInfo", UriKind.Relative), HttpMethod.Post, request);

		return result;
	}

	public async Task<Result<IEnumerable<EventInfo>>> GetCalendarEventsInPeriod(GetCalendarEventsInPeriodRequest request)
	{
		var result =
			await SendRequestAsync<GetCalendarEventsInPeriodRequest, IEnumerable<EventInfo>>(new Uri("Api/V1/CalendarEvent/GetInPeriod", UriKind.Relative), HttpMethod.Post, request);

		return result;
	}

	public async Task<Result<IEnumerable<EventInfo>>> GetEventsInPeriodForUser(GetEventsInPeriodForUserRequest request)
	{
		var result =
			await SendRequestAsync<GetEventsInPeriodForUserRequest, IEnumerable<EventInfo>>(new Uri("Api/V1/CalendarEvent/GetInPeriodForUser", UriKind.Relative), HttpMethod.Post, request);

		return result;
	}

	public async Task<Result<EventWithParticipants>> EditMyEventParticipationState(EditMyEventParticipationStateRequest request)
	{
		var result =
			await SendRequestAsync<EditMyEventParticipationStateRequest, EventWithParticipants>(new Uri("Api/V1/CalendarEvent/EditMyParticipationState", UriKind.Relative), HttpMethod.Post, request);

		return result;
	}

	public async Task<Result<GetPublicKeyResponse>> GetNotificationsPublicKey()
	{
		var result =
			await SendRequestAsync<GetPublicKeyResponse>(new Uri("Api/V1/Notification/GetPublicKey", UriKind.Relative), HttpMethod.Post);

		return result;
	}

	public async Task<Result<bool>> SubscribeToNotifications(AddNotificationSubscribeRequest request)
	{
		var result =
			await SendRequestAsync<AddNotificationSubscribeRequest, bool>(new Uri("Api/V1/Notification/Subscribe", UriKind.Relative), HttpMethod.Post, request);

		return result;
	}

	public async Task UnsubscribeFromNotifications(DeleteNotificationSubscribeRequest request)
	{
		await SendRequestAsync<DeleteNotificationSubscribeRequest, bool>(new Uri("Api/V1/Notification/Unsubscribe", UriKind.Relative), HttpMethod.Post, request);
	}

	protected override async Task RefreshTokensAsync()
	{
		string? refreshToken = await StorageService.GetRefreshTokenAsync();
		RefreshTokensRequest request = new(refreshToken!);

		var result =
			await SendAnonymousRequestAsync<RefreshTokensRequest, Tokens>(new Uri("Api/V1/User/Refresh", UriKind.Relative), HttpMethod.Post, request);

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