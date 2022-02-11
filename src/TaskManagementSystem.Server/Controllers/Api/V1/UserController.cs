using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> RegisterUserAsync(RegisterRequest request)
    {
        try
        {
            var registerResult = await userService.RegisterUserAsync(new RegisterData(request.Name, request.Email, request.Password));

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
    public async Task<IActionResult> LoginAsync(LoginRequest request)
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
    public IActionResult Refresh(RefreshTokensRequest request)
    {
        var tokenResult = tokenService.RefreshAccessToken(request.RefreshToken);

        if (!tokenResult.IsSuccess)
        {
            return Ok(new RefreshTokensResponse(false, null, tokenResult.ErrorDescription));
        }

        return Ok(new RefreshTokensResponse(true, tokenResult.Value, null));
    }

    [Authorize]
    [HttpPost("GetInfo")]
    public async Task<IActionResult> GetInfoAsync()
    {
        var idResult = tokenService.GetUserIdFromClaims(User);

        if (!idResult.IsSuccess)
        {
            return Ok(new GetUserInfoResponse(false, null, idResult.ErrorDescription));
        }

        var userResult = await userService.GetUserAsync(idResult.Value);

        if (!userResult.IsSuccess)
        {
            return Ok(new GetUserInfoResponse(false, null, userResult.ErrorDescription));
        }

        UserInfo userInfo = new(userResult.Value!.Name, userResult.Value.Email, userResult.Value.DateJoined);

        return Ok(new GetUserInfoResponse(true, userInfo, null));
    }
    
    [Authorize]
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest request)
    {
        request.AssertNotNull();
        
        var idResult = tokenService.GetUserIdFromClaims(User);

        if (!idResult.IsSuccess)
        {
            return Ok(new ChangePasswordResponse(false, null, idResult.ErrorDescription));
        }

        var userResult = await userService.ChangePasswordAsync(new ChangePasswordData(idResult.Value!, request.OldPassword, request.NewPassword));

        if (!userResult.IsSuccess)
        {
            return Ok(new ChangePasswordResponse(false, null, userResult.ErrorDescription));
        }
        
        UserInfo userInfo = new(userResult.Value!.Name, userResult.Value.Email, userResult.Value.DateJoined);
        
        return Ok(new ChangePasswordResponse(true, userInfo, null));
    }
    
        
    [Authorize]
    [HttpPost("ChangeInfo")]
    public async Task<IActionResult> ChangeInfoAsync(ChangeUserInfoRequest request)
    {
        request.AssertNotNull();
        
        var idResult = tokenService.GetUserIdFromClaims(User);

        if (!idResult.IsSuccess)
        {
            return Ok(new ChangeUserInfoResponse(false, null, idResult.ErrorDescription));
        }

        var userResult = await userService.ChangeUserInfoAsync(new ChangeUserInfoData(idResult.Value!, request.Name, request.Email));

        if (!userResult.IsSuccess)
        {
            return Ok(new ChangeUserInfoResponse(false, null, userResult.ErrorDescription));
        }
        
        UserInfo userInfo = new(userResult.Value!.Name, userResult.Value.Email, userResult.Value.DateJoined);
        
        return Ok(new ChangeUserInfoResponse(true, userInfo, null));
    }
}