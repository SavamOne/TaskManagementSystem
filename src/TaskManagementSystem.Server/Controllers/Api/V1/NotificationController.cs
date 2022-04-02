using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Server.Filters;
using TaskManagementSystem.Server.Services;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Controllers.Api.V1;

[Authorize]
[ApiController]
[ServiceFilter(typeof(ApiResponseExceptionFilter))]
[Route("Api/V1/[controller]")]
public class NotificationController : ControllerBase
{
	private readonly ITokenService tokenService;
	private readonly INotificationService notificationService;

	public NotificationController(ITokenService tokenService, INotificationService notificationService)
	{
		this.tokenService = tokenService;
		this.notificationService = notificationService;
	}

	[HttpPost("Subscribe")]
	public async Task<IActionResult> SubscribeAsync(AddNotificationSubscribeRequest request)
	{
		Guid userId = tokenService.GetUserIdFromClaims(User);

		await notificationService.AddSubscriptionAsync(new NotificationSubscription(userId, request.Url, request.P256dh, request.Auth));

		return Ok(true);
	}

	[HttpPost("Unsubscribe")]
	public async Task<IActionResult> Unsubscribe(AddNotificationSubscribeRequest request)
	{
		await notificationService.DeleteSubscriptionAsync(request.Url.AssertNotNullOrWhiteSpace());

		return Ok(true);
	}

	[HttpPost("GetPublicKey")]
	public IActionResult GetPublicKey()
	{
		string key = notificationService.GetPublicKey();
		return Ok(new GetPublicKeyResponse(key));
	}
}