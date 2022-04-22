using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
public class NotificationController : ControllerBase
{
	private readonly INotificationService notificationService;
	private readonly ITokenService tokenService;

	public NotificationController(ITokenService tokenService, INotificationService notificationService)
	{
		this.tokenService = tokenService;
		this.notificationService = notificationService;
	}

	/// <summary>
	///     Подписать устройство на получение уведомлений.
	/// </summary>
	/// <param name="request"><see cref="AddNotificationSubscribeRequest" />.</param>
	/// <returns><see cref="bool" />.</returns>
	/// <response code="200">Возвращает <see cref="bool" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(bool), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("Subscribe")]
	public async Task<IActionResult> SubscribeAsync([Required] AddNotificationSubscribeRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		await notificationService.AddSubscriptionAsync(new NotificationSubscription(userId, request.Url, request.P256dh, request.Auth));

		return Ok(true);
	}

	/// <summary>
	///     Отписать устройство от получения уведомлений.
	/// </summary>
	/// <param name="request">
	///     <see cref="DeleteNotificationSubscribeRequest" />
	/// </param>
	/// <returns><see cref="bool" />.</returns>
	/// <response code="200">Возвращает <see cref="bool" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(bool), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[HttpPost("Unsubscribe")]
	[AllowAnonymous]
	public async Task<IActionResult> Unsubscribe([Required] DeleteNotificationSubscribeRequest request)
	{
		await notificationService.DeleteSubscriptionAsync(request.Url.AssertNotNullOrWhiteSpace());

		return Ok(true);
	}

	/// <summary>
	///     Получить публичный ключ для подписки на уведомления.
	/// </summary>
	/// <returns><see cref="GetPublicKeyResponse" />.</returns>
	/// <response code="200">Возвращает <see cref="GetPublicKeyResponse" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(GetPublicKeyResponse), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[HttpPost("GetPublicKey")]
	public IActionResult GetPublicKey()
	{
		string key = notificationService.GetPublicKey();
		return Ok(new GetPublicKeyResponse(key));
	}
}