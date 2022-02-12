using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BusinessLogic.Models;
using TaskManagementSystem.BusinessLogic.Services;
using TaskManagementSystem.Server.Filters;
using TaskManagementSystem.Server.Services;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Controllers.Api.V1;

[ApiController]
[ServiceFilter(typeof(ApiResponseExceptionFilter))]
[Route("Api/V1/[controller]")]
public class UserController : ControllerBase
{
    private readonly ITokenService tokenService;
    private readonly IUserService userService;

    public UserController(
        IUserService userService,
        ITokenService tokenService)
    {
        this.userService = userService.AssertNotNull();
        this.tokenService = tokenService.AssertNotNull();
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUserAsync(RegisterRequest request)
    {
        request.AssertNotNull();

        User registeredUser = await userService.RegisterUserAsync(new RegisterData(request.Name, request.Email, request.Password));

        Tokens tokens = tokenService.GenerateAccessAndRefreshTokens(registeredUser);

        return Ok(tokens);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        request.AssertNotNull();

        User user = await userService.CheckUserCredentialsAsync(new LoginData(request.Email, request.Password));
        Tokens tokens = tokenService.GenerateAccessAndRefreshTokens(user);

        return Ok(tokens);
    }

    [HttpPost("Refresh")]
    public IActionResult Refresh(RefreshTokensRequest request)
    {
        request.AssertNotNull();

        Tokens tokens = tokenService.RefreshAccessToken(request.RefreshToken);

        return Ok(tokens);
    }

    [Authorize]
    [HttpPost("GetInfo")]
    public async Task<IActionResult> GetInfoAsync()
    {
        Guid idResult = tokenService.GetUserIdFromClaims(User);
        User user = await userService.GetUserAsync(idResult);

        return Ok(new UserInfo(user.Name, user.Email, user.DateJoined));
    }

    [Authorize]
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest request)
    {
        request.AssertNotNull();

        Guid id = tokenService.GetUserIdFromClaims(User);
        User user = await userService.ChangePasswordAsync(new ChangePasswordData(id, request.OldPassword, request.NewPassword));

        return Ok(new UserInfo(user.Name, user.Email, user.DateJoined));
    }


    [Authorize]
    [HttpPost("ChangeInfo")]
    public async Task<IActionResult> ChangeInfoAsync(ChangeUserInfoRequest request)
    {
        request.AssertNotNull();

        Guid id = tokenService.GetUserIdFromClaims(User);
        User user = await userService.ChangeUserInfoAsync(new ChangeUserInfoData(id, request.Name, request.Email));

        return Ok(new UserInfo(user.Name, user.Email, user.DateJoined));
    }
}