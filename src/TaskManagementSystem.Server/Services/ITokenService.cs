using System.Security.Claims;
using TaskManagementSystem.BusinessLogic.Models;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Services;

public interface ITokenService
{
    Task<Tokens> GenerateAccessAndRefreshTokensAsync(User user);

    Task<Tokens> RefreshAccessTokenAsync(string refreshToken);

    Guid GetUserIdFromClaims(ClaimsPrincipal principal);
}