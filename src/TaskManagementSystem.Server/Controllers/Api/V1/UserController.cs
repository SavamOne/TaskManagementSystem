using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BusinessLogic.Models;
using TaskManagementSystem.BusinessLogic.Services;
using TaskManagementSystem.Server.Services;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Controllers.Api.V1;

[ApiController]
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
    public async Task<IActionResult> RegisterUser(RegisterRequest request)
    {
        try
        {
            var registerResult = await userService.RegisterUserAsync(new RegisterData(request.Username, request.Email, request.Password));

            if (!registerResult.IsSuccess)
            {
                return Ok(new RegisterResponse(false, null, registerResult.ErrorDescription));
            }

            var tokenResult = tokenService.GenerateAccessAndRefreshTokens(registerResult.Value!);

            if (!tokenResult.IsSuccess)
            {
                return Ok(new RegisterResponse(false, null, tokenResult.ErrorDescription));
            }

            return Ok(new RegisterResponse(true, tokenResult.Value, null));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var checkResult = await userService.CheckUserCredentialsAsync(new LoginData(request.Email, request.Password));

        if (!checkResult.IsSuccess)
        {
            return Ok(new LoginResponse(false, null, checkResult.ErrorDescription));
        }


        var tokenResult = tokenService.GenerateAccessAndRefreshTokens(checkResult.Value!);

        if (!tokenResult.IsSuccess)
        {
            return Ok(new LoginResponse(false, null, tokenResult.ErrorDescription));
        }

        return Ok(new LoginResponse(true, tokenResult.Value, null));
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh(RefreshTokensRequest request)
    {
        var tokenResult = tokenService.RefreshAccessToken(request.RefreshToken);

        if (!tokenResult.IsSuccess)
        {
            return Ok(new RefreshTokensResponse(false, null, tokenResult.ErrorDescription));
        }

        return Ok(new RefreshTokensResponse(true, tokenResult.Value, null));
    }
}