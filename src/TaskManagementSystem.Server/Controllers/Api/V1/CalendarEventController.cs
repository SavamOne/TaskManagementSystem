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
	private readonly ITokenService tokenService;
	private readonly IUserService userService;
	private readonly ICalendarService calendarService;
	private readonly ICalendarEventService eventService;


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

		CalendarEvent result = await eventService.CreateEventAsync(new(userId,
			request.CalendarId,
			request.Name,
			request.Description,
			(EventType)request.Type,
			request.Place,
			request.StartTime,
			request.EndTime,
			request.IsPrivate));

		return Ok(new EventInfo(result.Id,
			result.CalendarId,
			result.Name,
			result.Description,
			(CalendarEventType)result.EventType,
			result.Place,
			result.StartTimeUtc,
			result.EndTimeUtc,
			result.IsPrivate,
			result.CreationTimeUtc));
	}

	[HttpPost("Delete")]
	public async Task<IActionResult> DeleteEventAsync(DeleteEventRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		await eventService.DeleteEventAsync(new(userId, request.EventId));

		return Ok();
	}

	[HttpPost("Edit")]
	public async Task<IActionResult> EditEventAsync(EditEventRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);
		
		CalendarEvent result = await eventService.EditEventAsync(new ChangeCalendarEventData(userId,
			request.EventId,
			request.Name,
			request.Description,
			(EventType)request.Type,
			request.Place,
			request.StartTime,
			request.EndTime,
			request.IsPrivate));

		return Ok(new EventInfo(result.Id,
			result.CalendarId,
			result.Name,
			result.Description,
			(CalendarEventType)result.EventType,
			result.Place,
			result.StartTimeUtc,
			result.EndTimeUtc,
			result.IsPrivate,
			result.CreationTimeUtc));
	}

	[HttpPost("AddParticipants")]
	public async Task<IActionResult> AddParticipantsAsync(AddEventParticipantsRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		CalendarEventWithParticipants result = await eventService.AddEventParticipant(new AddEventParticipantsData(userId,
			request.EventId,
			request.Participants
			   .Select(x =>
					new AddEventParticipantData(x.ParticipantId,
						(CalendarEventParticipantRole)x.Role))
			   .ToList()));

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

		return Ok(result.Select(x => new EventInfo(x.Id,
			x.CalendarId,
			x.Name,
			x.Description,
			(CalendarEventType)x.EventType,
			x.Place,
			x.StartTimeUtc,
			x.EndTimeUtc,
			x.IsPrivate,
			x.CreationTimeUtc)));
	}


	private EventWithParticipants Convert(CalendarEventWithParticipants eventWithParticipants)
	{
		return new EventWithParticipants(new EventInfo(eventWithParticipants.Event.Id,
				eventWithParticipants.Event.CalendarId,
				eventWithParticipants.Event.Name,
				eventWithParticipants.Event.Description,
				(CalendarEventType)eventWithParticipants.Event.EventType,
				eventWithParticipants.Event.Place,
				eventWithParticipants.Event.StartTimeUtc,
				eventWithParticipants.Event.EndTimeUtc,
				eventWithParticipants.Event.IsPrivate,
				eventWithParticipants.Event.CreationTimeUtc),
			eventWithParticipants.Participants.Select(x =>
					new EventParticipantUser(x.CalendarParticipant!.User!.Name,
						x.CalendarParticipant!.User!.Email,
						x.Id,
						x.CalendarParticipantId,
						x.CalendarParticipant!.User!.Id,
						x.EventId,
						x.CalendarParticipant!.CalendarId,
						(EventParticipantRole)x.Role))
			   .ToList());
	}
}