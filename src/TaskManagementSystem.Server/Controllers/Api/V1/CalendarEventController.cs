using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;
using TaskManagementSystem.BusinessLogic.Services;
using TaskManagementSystem.Server.Filters;
using TaskManagementSystem.Server.Services;
using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Server.Controllers.Api.V1;

[Authorize]
[ApiController]
[ServiceFilter(typeof(ApiResponseExceptionFilter))]
[Route("Api/V1/[controller]")]
public class CalendarEventController : ControllerBase
{
	private readonly ICalendarEventService eventService;
	private readonly ILogger<CalendarEventController> logger;
	private readonly ITokenService tokenService;

	public CalendarEventController(ITokenService tokenService, ICalendarEventService eventService, ILogger<CalendarEventController> logger)
	{
		this.tokenService = tokenService;
		this.eventService = eventService;
		this.logger = logger;
	}

	/// <summary>
	///     Создать событие в календаре.
	/// </summary>
	/// <param name="request"><see cref="CreateEventRequest" />.</param>
	/// <returns><see cref="EventInfo" />.</returns>
	/// <response code="200">Возвращает <see cref="EventInfo" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(EventInfo), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("Create")]
	public async Task<IActionResult> CreateEventAsync([Required] CreateEventRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		CalendarEvent result = await eventService.CreateEventAsync(Convert(request, userId));

		return Ok(Convert(result));
	}

	/// <summary>
	///     Удалить событие в календаре.
	/// </summary>
	/// <param name="request"><see cref="DeleteEventRequest" />.</param>
	/// <returns><see cref="bool" />.</returns>
	/// <response code="200">Возвращает <see cref="bool" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(bool), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("Delete")]
	public async Task<IActionResult> DeleteEventAsync([Required] DeleteEventRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		await eventService.DeleteEventAsync(new DeleteEventData(userId, request.EventId));

		return Ok(true);
	}

	/// <summary>
	///     Редактировать событие.
	/// </summary>
	/// <param name="request"><see cref="EditEventRequest" />.</param>
	/// <returns><see cref="EventInfo" />.</returns>
	/// <response code="200">Возвращает <see cref="EventInfo" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(EventInfo), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("Edit")]
	public async Task<IActionResult> EditEventAsync([Required] EditEventRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		CalendarEvent result = await eventService.ChangeEventAsync(Convert(request, userId));

		return Ok(Convert(result));
	}

	/// <summary>
	///     Добавить участников в событие.
	/// </summary>
	/// <param name="request"><see cref="AddEventParticipantsRequest" />.</param>
	/// <returns><see cref="EventWithParticipants" />.</returns>
	/// <response code="200">Возвращает <see cref="EventWithParticipants" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(EventWithParticipants), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("AddParticipants")]
	public async Task<IActionResult> AddParticipantsAsync([Required] AddEventParticipantsRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		CalendarEventWithParticipants result = await eventService.AddEventParticipant(Convert(request, userId));

		return Ok(Convert(result));
	}

	/// <summary>
	///     Изменить роль/удалить участников.
	/// </summary>
	/// <param name="request"><see cref="ChangeEventParticipantsRequest" />.</param>
	/// <returns><see cref="EventWithParticipants" />.</returns>
	/// <response code="200">Возвращает <see cref="EventWithParticipants" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(EventWithParticipants), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("ChangeParticipants")]
	public async Task<IActionResult> ChangeParticipantsAsync([Required] ChangeEventParticipantsRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		CalendarEventWithParticipants result = await eventService.ChangeEventParticipants(Convert(request, userId));

		return Ok(Convert(result));
	}

	/// <summary>
	///     Получить полную информацию о событии.
	/// </summary>
	/// <param name="request"><see cref="GetEventInfoRequest" />.</param>
	/// <returns><see cref="EventWithParticipants" />.</returns>
	/// <response code="200">Возвращает <see cref="EventWithParticipants" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(EventWithParticipants), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("GetInfo")]
	public async Task<IActionResult> GetEventInfoAsync([Required] GetEventInfoRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		CalendarEventWithParticipants result = await eventService.GetEventInfo(new GetEventInfoData(userId, request.EventId));

		return Ok(Convert(result));
	}

	/// <summary>
	///     Получить список событий в периоде для конкретного календаря.
	/// </summary>
	/// <param name="request"><see cref="GetCalendarEventsInPeriodRequest" />.</param>
	/// <returns>Коллекция <see cref="EventInfo" />.</returns>
	/// <response code="200">Возвращает коллекцию <see cref="EventInfo" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(IEnumerable<EventInfo>), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("GetInPeriod")]
	public async Task<IActionResult> GetCalendarEventsInPeriodAsync([Required] GetCalendarEventsInPeriodRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		Stopwatch stopWatch = Stopwatch.StartNew();
		var result = await eventService.GetCalendarEventsInPeriodAsync(new GetCalendarEventsInPeriodData(userId, request.CalendarId, request.StartPeriod, request.EndPeriod));
		
		logger.LogInformation("{0} processed in {1:g}", nameof(GetCalendarEventsInPeriodAsync), stopWatch.Elapsed);
		
		return Ok(result.Select(Convert).ToList());
	}

	/// <summary>
	///     Получить список событий в периоде для пользователя.
	/// </summary>
	/// <param name="request"><see cref="GetEventsInPeriodForUserRequest" />.</param>
	/// <returns>Коллекция <see cref="EventInfo" />.</returns>
	/// <response code="200">Возвращает коллекцию <see cref="EventInfo" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(IEnumerable<EventInfo>), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("GetInPeriodForUser")]
	public async Task<IActionResult> GetEventsInPeriodForUserAsync([Required] GetEventsInPeriodForUserRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		Stopwatch stopWatch = Stopwatch.StartNew();
		var result = await eventService.GetEventsForUserInPeriodAsync(new GetEventsInPeriodForUserData(userId, request.StartPeriod, request.EndPeriod));
		
		logger.LogInformation("{0} processed in {1:g}", nameof(GetCalendarEventsInPeriodAsync), stopWatch.Elapsed);
		
		return Ok(result.Select(Convert).ToList());
	}
	
	/// <summary>
	///     Обновить состояние участия в событии.
	/// </summary>
	/// <param name="request"><see cref="ChangeMyEventParticipationStateRequest" />.</param>
	/// <returns><see cref="bool" />.</returns>
	/// <response code="200">Возвращает <see cref="bool" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(bool), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("ChangeMyParticipationState")]
	public async Task<IActionResult> ChangeMyParticipationStateAsync([Required] ChangeMyEventParticipationStateRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);
		
		await eventService.ChangeParticipantState(new ChangeParticipantStateData(userId, request.EventId, (CalendarEventParticipantState)request.ParticipantState, request.NotifyBefore));

		return Ok(true);
	}

	private static EventWithParticipants Convert(CalendarEventWithParticipants eventWithParticipants)
	{
		return new EventWithParticipants(Convert(eventWithParticipants.Event),
			eventWithParticipants.Participants.Select(Convert).ToList(),
			eventWithParticipants.CanUserEditEvent,
			eventWithParticipants.CanUserEditParticipants,
			eventWithParticipants.CanUserDeleteEvent,
			(EventParticipantState?)eventWithParticipants.ParticipationState,
			eventWithParticipants.NotifyBefore,
			Convert(eventWithParticipants.RecurrentEventSettings));
	}

	private static RecurrentSettings? Convert(RecurrentEventSettings? recurrentEventSettings)
	{
		if (recurrentEventSettings is null)
		{
			return null;
		}

		return new RecurrentSettings((EventRepeatType)recurrentEventSettings.RepeatType,
			recurrentEventSettings.DayOfWeeks,
			recurrentEventSettings.RepeatCount,
			recurrentEventSettings.UntilUtc);
	}

	private static EventParticipantUser Convert(CalendarEventParticipant x)
	{
		return new EventParticipantUser(x.CalendarParticipant!.User!.Name,
			x.CalendarParticipant!.User!.Email,
			x.Id,
			x.CalendarParticipantId,
			x.CalendarParticipant!.User!.Id,
			x.EventId,
			x.CalendarParticipant!.CalendarId,
			(EventParticipantRole)x.Role,
			(EventParticipantState)x.State);
	}

	private static EventInfo Convert(CalendarEvent x)
	{
		return new EventInfo(x.Id,
			x.CalendarId,
			x.Name,
			x.Description,
			(CalendarEventType)x.EventType,
			x.Place,
			x.StartTimeUtc,
			x.EndTimeUtc,
			x.IsPrivate,
			x.CreationTimeUtc,
			x.IsRepeated,
			x.RepeatNum);
	}

	private static AddCalendarEventData Convert(CreateEventRequest request, Guid userId)
	{
		return new AddCalendarEventData(userId,
			request.CalendarId,
			request.Name,
			request.Description,
			(EventType)request.Type,
			request.Place,
			request.StartTime,
			request.EndTime,
			request.IsPrivate,
			Convert(request.RecurrentSettings));
	}

	private static ChangeCalendarEventData Convert(EditEventRequest request, Guid userId)
	{
		return new ChangeCalendarEventData(userId,
			request.EventId,
			request.IsRepeated,
			request.Name,
			request.Description,
			(EventType?)request.Type,
			request.Place,
			request.StartTime,
			request.EndTime,
			request.IsPrivate,
			Convert(request.RecurrentSettings));
	}

	private static AddRecurrentSettingsData? Convert(RecurrentSettings? request)
	{
		if (request is null)
		{
			return null;
		}
		
		return new AddRecurrentSettingsData((RepeatType)request.RepeatType,
			request.DayOfWeeks,
			request.Until,
			request.Count);
	}

	private static AddEventParticipantsData Convert(AddEventParticipantsRequest request, Guid userId)
	{
		return new AddEventParticipantsData(userId,
			request.EventId,
			request.Participants.Select(x =>
					new AddEventParticipantData(x.ParticipantId,
						(CalendarEventParticipantRole)x.Role))
			   .ToList());
	}

	private static ChangeEventParticipantsData Convert(ChangeEventParticipantsRequest request, Guid userId)
	{
		return new ChangeEventParticipantsData(userId,
			request.EventId,
			request.Participants.Select(x =>
					new ChangeEventParticipantData(x.EventParticipantId, (CalendarEventParticipantRole?)x.Role, x.Delete))
			   .ToList());
	}
}