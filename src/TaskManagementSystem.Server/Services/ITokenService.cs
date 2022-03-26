using System.Security.Claims;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Services;

public interface ITokenService
{
	Task<Tokens> GenerateAccessAndRefreshTokensAsync(User user);

	Task<Tokens> RefreshAccessTokenAsync(string refreshToken);

	Task RemoveTokenAsync(string refreshToken);

	Guid GetUserIdFromClaims(ClaimsPrincipal principal);
}