namespace TaskManagementSystem.Shared.Models;

public class Tokens
{
    public Tokens(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public string AccessToken { get; }
    
    public string RefreshToken { get; }
}