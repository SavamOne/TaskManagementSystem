using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;
using TaskManagementSystem.BusinessLogic.Services;
using TaskManagementSystem.Server.Filters;
using TaskManagementSystem.Server.Services;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Server.Controllers.Api.V1;

[Authorize]
[ApiController]
[ServiceFilter(typeof(ApiResponseExceptionFilter))]
[Route("Api/V1/[controller]")]
public class CalendarController : ControllerBase
{
	private readonly ICalendarService calendarService;
	private readonly ITokenService tokenService;

	public CalendarController(ITokenService tokenService, ICalendarService calendarService)
	{
		this.tokenService = tokenService;
		this.calendarService = calendarService;
	}
	
	/// <summary>
	/// Получить список календарей пользователей.
	/// </summary>
	/// <returns>Коллекция <see cref="CalendarInfo"/>.</returns>
	/// <response code="200">Возвращает коллекцию <see cref="CalendarInfo"/>.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject"/>.</response>
	[HttpPost("GetMyList")]
	[ProducesResponseType(typeof(IEnumerable<CalendarInfo>), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> GetUserCalendarListAsync()
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		var result = await calendarService.GetUserCalendars(userId);

		return Ok(result.Select(x => new CalendarInfo(x.Id, x.Name, x.Description, x.CreationDateUtc)));
	}

	/// <summary>
	/// Создать календарь.
	/// </summary>
	/// <param name="request"><see cref="CreateCalendarRequest"/>.</param>
	/// <returns><see cref="CalendarInfo"/>.</returns>
	/// <response code="200">Возвращает <see cref="CalendarInfo"/>.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject"/>.</response>
	[ProducesResponseType(typeof(CalendarInfo), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject),StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("Create")]
	public async Task<IActionResult> CreateCalendarAsync([Required] CreateCalendarRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		Calendar result = await calendarService.CreateCalendarAsync(new CreateCalendarData(userId, request.Name, request.Description));

		return Ok(new CalendarInfo(result.Id, result.Name, result.Description, result.CreationDateUtc));
	}

	/// <summary>
	/// Отредактировать календарь.
	/// </summary>
	/// <param name="request"><see cref="EditCalendarRequest"/>.</param>
	/// <returns><see cref="CalendarInfo"/>.</returns>
	/// <response code="200">Возвращает <see cref="CalendarInfo"/>.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject"/>.</response>
	[ProducesResponseType(typeof(CalendarInfo), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject),StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("Edit")]
	public async Task<IActionResult> EditCalendarAsync([Required] EditCalendarRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		Calendar result = await calendarService.EditCalendarAsync(new EditCalendarData(userId, request.CalendarId, request.Name, request.Description));

		return Ok(new CalendarInfo(result.Id, result.Name, result.Description, result.CreationDateUtc));
	}

	/// <summary>
	/// Добавить участников календаря.
	/// </summary>
	/// <param name="request"><see cref="AddCalendarParticipantsRequest"/>.</param>
	/// <returns><see cref="CalendarWithParticipantUsers"/>.</returns>
	/// <response code="200">Возвращает <see cref="CalendarWithParticipantUsers"/>.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject"/>.</response>
	[ProducesResponseType(typeof(CalendarWithParticipantUsers), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject),StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("AddParticipants")]
	public async Task<IActionResult> AddParticipantsAsync([Required] AddCalendarParticipantsRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		CalendarWithParticipants result = await calendarService.AddParticipantsAsync(new AddCalendarParticipantsData(userId,
			request.CalendarId,
			request.Users
			   .Select(x => new AddCalendarParticipantData(x.UserId, (CalendarRole)x.Role))
			   .ToHashSet()));

		return Ok(await ConvertAsync(result));
	}

	/// <summary>
	/// Изменить роль участников календаря.
	/// </summary>
	/// <param name="request"><see cref="ChangeCalendarParticipantsRoleRequest"/>.</param>
	/// <returns><see cref="CalendarWithParticipantUsers"/>.</returns>
	/// <response code="200">Возвращает <see cref="CalendarWithParticipantUsers"/>.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject"/>.</response>
	[ProducesResponseType(typeof(CalendarWithParticipantUsers), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject),StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("ChangeParticipantsRole")]
	public async Task<IActionResult> ChangeParticipantsRoleAsync([Required] ChangeCalendarParticipantsRoleRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		CalendarWithParticipants result = await calendarService.ChangeParticipantRoleAsync(new ChangeParticipantsRoleData(userId,
			request.CalendarId,
			request.Participants
			   .Select(x => new ChangeParticipantRoleData(x.ParticipantId, (CalendarRole)x.Role))
			   .ToHashSet()));

		return Ok(await ConvertAsync(result));
	}

	/// <summary>
	/// Удалить участников календаря.
	/// </summary>
	/// <param name="request"><see cref="DeleteParticipantsRequest"/>.</param>
	/// <returns><see cref="CalendarWithParticipantUsers"/>.</returns>
	/// <response code="200">Возвращает <see cref="CalendarWithParticipantUsers"/>.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject"/>.</response>
	[ProducesResponseType(typeof(CalendarWithParticipantUsers), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject),StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("DeleteParticipants")]
	public async Task<IActionResult> DeleteParticipantsAsync([Required] DeleteParticipantsRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		CalendarWithParticipants result = await calendarService.DeleteParticipantsAsync(new DeleteParticipantsData(userId,
			request.CalendarId,
			request.ParticipantIds.ToHashSet()));

		return Ok(await ConvertAsync(result));
	}

	/// <summary>
	/// Получить информацию о календаре.
	/// </summary>
	/// <param name="request"><see cref="GetCalendarInfoRequest"/>.</param>
	/// <returns><see cref="CalendarWithParticipantUsers"/>.</returns>
	/// <response code="200">Возвращает <see cref="CalendarWithParticipantUsers"/>.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject"/>.</response>
	[ProducesResponseType(typeof(CalendarWithParticipantUsers), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject),StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("GetInfo")]
	public async Task<IActionResult> GetCalendarInfoAsync([Required] GetCalendarInfoRequest request)
	{
		CalendarWithParticipants result = await calendarService.GetCalendarInfoAsync(request.CalendarId);

		return Ok(await ConvertAsync(result));
	}

	/// <summary>
	/// Получить участников календаря по фильтру.
	/// </summary>
	/// <param name="request"><see cref="GetCalendarParticipantsByFilterRequest"/>.</param>
	/// <returns>Коллекция <see cref="CalendarParticipantUser"/>.</returns>
	/// <response code="200">Возвращает коллекцию <see cref="CalendarParticipantUser"/>.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject"/>.</response>
	[ProducesResponseType(typeof(IEnumerable<CalendarParticipantUser>), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject),StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("GetParticipantsByFilter")]
	public async Task<IActionResult> GetParticipantsByFilterAsync([Required] GetCalendarParticipantsByFilterRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		request.AssertNotNull();

		var result = await calendarService.GetParticipantsByFilter(new GetCalendarParticipantsByFilter(userId, request.CalendarId, request.Filter));

		var participantsUsers = result.Select(participant => new CalendarParticipantUser(participant.Id,
				participant.CalendarId,
				participant.UserId,
				participant.JoinDateUtc,
				(CalendarParticipantRole)participant.Role,
				participant.User!.Name,
				participant.User.Email,
				participant.User.DateJoinedUtc))
		   .ToList();

		return Ok(participantsUsers);
	}

	private async Task<CalendarWithParticipantUsers> ConvertAsync([Required] CalendarWithParticipants request)
	{
		var participantsUsers = request.Participants.Select(participant => new CalendarParticipantUser(participant.Id,
			participant.CalendarId,
			participant.UserId,
			participant.JoinDateUtc,
			(CalendarParticipantRole)participant.Role,
			participant.User!.Name,
			participant.User.Email,
			participant.User.DateJoinedUtc));

		return new CalendarWithParticipantUsers(new CalendarInfo(request.Calendar.Id,
				request.Calendar.Name,
				request.Calendar.Description,
				request.Calendar.CreationDateUtc),
			participantsUsers);
	}
}