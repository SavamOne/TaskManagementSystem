using System.Security.Claims;
using TaskManagementSystem.BusinessLogic.Models;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Services;

public interface ITokenService
{
    Result<Tokens> GenerateAccessAndRefreshTokens(User? user);

    Result<Tokens> RefreshAccessToken(string? refreshToken);

    Result<Guid> GetUserIdFromClaims(ClaimsPrincipal? principal);
}