using System.ComponentModel.DataAnnotations;
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
	private readonly ITokenService tokenService;

	public CalendarEventController(ITokenService tokenService, ICalendarEventService eventService)
	{
		this.tokenService = tokenService;
		this.eventService = eventService;
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
	///     Получить спискок событий в периоде.
	/// </summary>
	/// <param name="request"><see cref="GetEventsInPeriodRequest" />.</param>
	/// <returns>Коллекция <see cref="EventInfo" />.</returns>
	/// <response code="200">Возвращает коллекцию <see cref="EventInfo" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(IEnumerable<EventInfo>), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("GetInPeriod")]
	public async Task<IActionResult> GetEventsInPeriodAsync([Required] GetEventsInPeriodRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		var result = await eventService.GetEventsInPeriodAsync(new GetEventsInPeriodData(userId, request.CalendarId, request.StartPeriod, request.EndPeriod));

		return Ok(result.Select(Convert).ToList());
	}


	private static EventWithParticipants Convert(CalendarEventWithParticipants eventWithParticipants)
	{
		return new EventWithParticipants(Convert(eventWithParticipants.Event),
			eventWithParticipants.Participants.Select(Convert).ToList(),
			eventWithParticipants.CanUserEditEvent,
			eventWithParticipants.CanUserEditParticipants,
			eventWithParticipants.CanUserDeleteEvent);
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
			(EventParticipantRole)x.Role);
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
			x.CreationTimeUtc);
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
			request.IsPrivate);
	}

	private static ChangeCalendarEventData Convert(EditEventRequest request, Guid userId)
	{
		return new ChangeCalendarEventData(userId,
			request.EventId,
			request.Name,
			request.Description,
			(EventType)request.Type,
			request.Place,
			request.StartTime,
			request.EndTime,
			request.IsPrivate);
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