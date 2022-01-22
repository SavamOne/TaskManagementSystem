using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

public class Tokens
{
    public Tokens(string accessToken, string refreshToken)
    {
        AccessToken = accessToken.AssertNotNullOrWhiteSpace();
        RefreshToken = refreshToken.AssertNotNullOrWhiteSpace();
    }

    public string AccessToken { get; }

    public string RefreshToken { get; }
}