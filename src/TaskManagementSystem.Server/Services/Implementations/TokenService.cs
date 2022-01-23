using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSystem.BusinessLogic.Models;
using TaskManagementSystem.Server.Options;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Services.Implementations;

public class TokenService : ITokenService
{
    private static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler = new();

    private readonly IOptions<JwtOptions> options;

    private readonly List<(User User, string RefreshToken)> refreshTokens = new();
    private readonly TokenValidationParameters refreshTokenValidationParams;

    public TokenService(IOptions<JwtOptions> options)
    {
        this.options = options;
        refreshTokenValidationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = options.Value.Issuer,
            ValidateAudience = true,
            ValidAudience = options.Value.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = options.Value.SymmetricRefreshKey,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    public Result<Tokens> GenerateAccessAndRefreshTokens(User user)
    {
        user.AssertNotNull();

        Tokens tokens = GenerateTokens(user);

        refreshTokens.Add(( user, tokens.RefreshToken ));

        return Result<Tokens>.Success(tokens);
    }

    public Result<Tokens> RefreshAccessToken(string refreshToken)
    {
        refreshToken.AssertNotNullOrWhiteSpace();

        if (!ValidateRefreshToken(refreshToken))
        {
            return Result<Tokens>.Error("Refresh token is incorrect");
        }

        (User User, string Token) tuple = refreshTokens.FirstOrDefault(x => string.Equals(x.RefreshToken, refreshToken));
        if (tuple.User is null)
        {
            return Result<Tokens>.Error("How do you got this token?");
        }

        Tokens tokens = GenerateTokens(tuple.User);

        refreshTokens.Remove(tuple);

        tuple.Token = tokens.RefreshToken;

        refreshTokens.Add(tuple);

        return Result<Tokens>.Success(tokens);
    }

    private Tokens GenerateTokens(User user)
    {
        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.NameId, Guid.Empty.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Username)
        };

        string accessToken = Generate(options.Value.SymmetricAccessKey,
        options.Value.AccessTokenExpirationMinutes,
        claims);
        string refreshToken = Generate(options.Value.SymmetricRefreshKey,
        options.Value.RefreshTokenExpirationMinutes);

        return new Tokens(accessToken, refreshToken);
    }

    private bool ValidateRefreshToken(string refreshToken)
    {
        try
        {
            JwtSecurityTokenHandler.ValidateToken(refreshToken, refreshTokenValidationParams, out SecurityToken _);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private string Generate(SecurityKey secretKey, int expirationMinutes, IEnumerable<Claim>? claims = null)
    {
        SigningCredentials credentials = new(secretKey, SecurityAlgorithms.HmacSha256);
        DateTime nowDate = DateTime.UtcNow;
        JwtSecurityToken securityToken = new(options.Value.Issuer,
        options.Value.Audience,
        claims,
        nowDate,
        nowDate.AddMinutes(expirationMinutes),
        credentials);
        return JwtSecurityTokenHandler.WriteToken(securityToken);
    }
}