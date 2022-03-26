using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;
using TaskManagementSystem.BusinessLogic.Services;
using TaskManagementSystem.Server.Filters;
using TaskManagementSystem.Server.Services;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Controllers.Api.V1;

[Authorize]
[ApiController]
[ServiceFilter(typeof(ApiResponseExceptionFilter))]
[Route("Api/V1/[controller]")]
public class CalendarEventController : ControllerBase
{
	private readonly ICalendarService calendarService;
	private readonly ICalendarEventService eventService;
	private readonly ITokenService tokenService;
	private readonly IUserService userService;


	public CalendarEventController(ITokenService tokenService,
		IUserService userService,
		ICalendarService calendarService,
		ICalendarEventService eventService)
	{
		this.tokenService = tokenService;
		this.userService = userService;
		this.calendarService = calendarService;
		this.eventService = eventService;
	}

	[HttpPost("Create")]
	public async Task<IActionResult> CreateEventAsync(CreateEventRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		CalendarEvent result = await eventService.CreateEventAsync(Convert(request, userId));

		return Ok(Convert(result));
	}

	[HttpPost("Delete")]
	public async Task<IActionResult> DeleteEventAsync(DeleteEventRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		await eventService.DeleteEventAsync(new DeleteEventData(userId, request.EventId));

		return Ok(true);
	}

	[HttpPost("Edit")]
	public async Task<IActionResult> EditEventAsync(EditEventRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		CalendarEvent result = await eventService.ChangeEventAsync(Convert(request, userId));

		return Ok(Convert(result));
	}

	[HttpPost("AddParticipants")]
	public async Task<IActionResult> AddParticipantsAsync(AddEventParticipantsRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		CalendarEventWithParticipants result = await eventService.AddEventParticipant(Convert(request, userId));

		return Ok(Convert(result));
	}

	[HttpPost("ChangeParticipants")]
	public async Task<IActionResult> ChangeParticipantsAsync(ChangeEventParticipantsRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		CalendarEventWithParticipants result = await eventService.ChangeEventParticipants(Convert(request, userId));

		return Ok(Convert(result));
	}

	[HttpPost("GetInfo")]
	public async Task<IActionResult> GetEventInfoAsync(GetEventInfoRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		CalendarEventWithParticipants result = await eventService.GetEventInfo(new GetEventInfoData(userId, request.EventId));

		return Ok(Convert(result));
	}

	[HttpPost("GetInPeriod")]
	public async Task<IActionResult> GetEventsInPeriodAsync(GetEventsInPeriodRequest request)
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