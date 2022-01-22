using TaskManagementSystem.BusinessLogic.Models;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Services;

public interface ITokenService
{
    Result<Tokens> GenerateAccessAndRefreshTokens(User user);

    Result<Tokens> RefreshAccessToken(string refreshToken);
}