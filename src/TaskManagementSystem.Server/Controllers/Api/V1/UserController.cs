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

[ApiController]
[ServiceFilter(typeof(ApiResponseExceptionFilter))]
[Route("Api/V1/[controller]")]
public class UserController : ControllerBase
{
	private readonly ITokenService tokenService;
	private readonly IUserService userService;

	public UserController(IUserService userService, ITokenService tokenService)
	{
		this.userService = userService.AssertNotNull();
		this.tokenService = tokenService.AssertNotNull();
	}

	/// <summary>
	///     Зарегистрироваться.
	/// </summary>
	/// <param name="request"><see cref="RegisterRequest" />.</param>
	/// <returns><see cref="Tokens" />.</returns>
	/// <response code="200">Возвращает <see cref="Tokens" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(Tokens), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[HttpPost("Register")]
	public async Task<IActionResult> RegisterUserAsync([Required] RegisterRequest request)
	{
		request.AssertNotNull();

		User registeredUser = await userService.RegisterUserAsync(new RegisterData(request.Name, request.Email, request.Password));
		Tokens tokens = await tokenService.GenerateAccessAndRefreshTokensAsync(registeredUser);

		return Ok(tokens);
	}

	/// <summary>
	///     Авторизоваться.
	/// </summary>
	/// <param name="request"><see cref="LoginRequest" />.</param>
	/// <returns><see cref="Tokens" />.</returns>
	/// <response code="200">Возвращает <see cref="Tokens" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(Tokens), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[HttpPost("Login")]
	public async Task<IActionResult> LoginAsync([Required] LoginRequest request)
	{
		request.AssertNotNull();

		User user = await userService.CheckUserCredentialsAsync(new LoginData(request.Email, request.Password));
		Tokens tokens = await tokenService.GenerateAccessAndRefreshTokensAsync(user);

		return Ok(tokens);
	}

	/// <summary>
	///     Обновить refresh-токен.
	/// </summary>
	/// <param name="request"><see cref="RefreshTokensRequest" />.</param>
	/// <returns><see cref="Tokens" />.</returns>
	/// <response code="200">Возвращает <see cref="Tokens" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(Tokens), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[HttpPost("Refresh")]
	public async Task<IActionResult> RefreshAsync([Required] RefreshTokensRequest request)
	{
		request.AssertNotNull();

		Tokens tokens = await tokenService.RefreshAccessTokenAsync(request.RefreshToken);

		return Ok(tokens);
	}

	/// <summary>
	///     Получить информацию о пользователе.
	/// </summary>
	/// <returns><see cref="UserInfo" />.</returns>
	/// <response code="200">Возвращает <see cref="UserInfo" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[Authorize]
	[HttpPost("GetInfo")]
	public async Task<IActionResult> GetInfoAsync()
	{
		Guid id = tokenService.GetUserIdFromClaims(User);

		User user = await userService.GetUserAsync(id);

		return Ok(Convert(user));
	}

	/// <summary>
	///     Получить информацию о пользователе по Id.
	/// </summary>
	/// <param name="request"><see cref="GetUserInfoByIdRequest" />.</param>
	/// <returns><see cref="UserInfo" />.</returns>
	/// <response code="200">Возвращает <see cref="UserInfo" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[Authorize]
	[HttpPost("GetInfoById")]
	public async Task<IActionResult> GetInfoAsync([Required] GetUserInfoByIdRequest request)
	{
		request.AssertNotNull();

		User user = await userService.GetUserAsync(request.UserId);

		return Ok(Convert(user));
	}

	/// <summary>
	///     Изменить пароль пользователя.
	/// </summary>
	/// <param name="request"><see cref="ChangePasswordRequest" />.</param>
	/// <returns><see cref="UserInfo" />.</returns>
	/// <response code="200">Возвращает <see cref="UserInfo" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[Authorize]
	[HttpPost("ChangePassword")]
	public async Task<IActionResult> ChangePasswordAsync([Required] ChangePasswordRequest request)
	{
		request.AssertNotNull();

		Guid id = tokenService.GetUserIdFromClaims(User);
		User user = await userService.ChangePasswordAsync(new ChangePasswordData(id, request.OldPassword, request.NewPassword));

		return Ok(Convert(user));
	}

	/// <summary>
	///     Изменить информацию о себе.
	/// </summary>
	/// <param name="request"><see cref="EditUserInfoRequest" />.</param>
	/// <returns><see cref="UserInfo" />.</returns>
	/// <response code="200">Возвращает <see cref="UserInfo" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[Authorize]
	[HttpPost("EditInfo")]
	public async Task<IActionResult> EditInfoAsync([Required] EditUserInfoRequest request)
	{
		request.AssertNotNull();

		Guid id = tokenService.GetUserIdFromClaims(User);
		User user = await userService.EditUserInfoAsync(new EditUserInfoData(id, request.Name, request.Email, request.Position, request.Department));

		return Ok(Convert(user));
	}

	/// <summary>
	///     Получить пользователей по фильтру.
	/// </summary>
	/// <param name="request"><see cref="GetUserInfosByFilterRequest" />.</param>
	/// <returns>Коллекция <see cref="UserInfo" />.</returns>
	/// <response code="200">Возвращает коллекцию <see cref="UserInfo" />.</response>
	/// <response code="400">Возвращает <see cref="ErrorObject" />.</response>
	[ProducesResponseType(typeof(IEnumerable<UserInfo>), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(ErrorObject), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[Authorize]
	[HttpPost("GetByFilter")]
	public async Task<IActionResult> GetUsersByFilterAsync([Required] GetUserInfosByFilterRequest request)
	{
		request.AssertNotNull();

		var result = await userService.GetUsersByFilter(request.Filter);

		return Ok(result.Select(Convert));
	}

	private static UserInfo Convert(User user)
	{
		return new UserInfo(user.Id,
			user.Name,
			user.Email,
			user.RegisterDateUtc,
			user.Position,
			user.Department);
	}
}