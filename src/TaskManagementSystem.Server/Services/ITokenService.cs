using System.Security.Claims;
using TaskManagementSystem.BusinessLogic.Models;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Services;

public interface ITokenService
{
    Tokens GenerateAccessAndRefreshTokens(User user);

    Tokens RefreshAccessToken(string refreshToken);

    Guid GetUserIdFromClaims(ClaimsPrincipal principal);
}