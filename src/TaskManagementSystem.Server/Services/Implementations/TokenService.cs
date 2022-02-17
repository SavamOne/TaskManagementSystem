using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSystem.BusinessLogic.Exceptions;
using TaskManagementSystem.BusinessLogic.Models;
using TaskManagementSystem.BusinessLogic.Services;
using TaskManagementSystem.Server.Dal.Repositories;
using TaskManagementSystem.Server.Options;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Services.Implementations;

public class TokenService : ITokenService
{
    private static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler = new();

    private readonly IOptions<JwtOptions> options;
    private readonly IUserService userService;
    private readonly IRefreshTokenRepository tokenRepository;
    private readonly TokenValidationParameters refreshTokenValidationParams;

    // TODO: Переделать, чтобы не тянуть userService
    public TokenService(IOptions<JwtOptions> options, IUserService userService, IRefreshTokenRepository tokenRepository)
    {
        this.options = options;
        this.userService = userService;
        this.tokenRepository = tokenRepository;

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

    public async Task<Tokens> GenerateAccessAndRefreshTokensAsync(User user)
    {
        user.AssertNotNull();

        Tokens tokens = GenerateTokens(user);

        await tokenRepository.InsertForUserAsync(user.Id, tokens.RefreshToken);
        return tokens;
    }

    public async Task<Tokens> RefreshAccessTokenAsync(string refreshToken)
    {
        refreshToken.AssertNotNullOrWhiteSpace();

        if (!TryValidateRefreshToken(refreshToken, out ClaimsPrincipal? principal))
        {
            throw new BusinessLogicException("Refresh token is incorrect or outdated");
        }

        Guid userId = GetUserIdFromClaims(principal!);
        User user = await userService.GetUserAsync(userId);
        Tokens tokens = GenerateTokens(user);

        await tokenRepository.UpdateForUserAsync(userId, refreshToken, tokens.RefreshToken);
        return tokens;
    }

    public Guid GetUserIdFromClaims(ClaimsPrincipal principal)
    {
        principal.AssertNotNull();

        string? idClaim = principal.FindFirstValue(JwtRegisteredClaimNames.NameId);

        if (!Guid.TryParse(idClaim, out Guid id))
        {
            throw new ApplicationException($"Claim {JwtRegisteredClaimNames.NameId} does not exists or is incorrect GUID");
        }

        return id;
    }

    private Tokens GenerateTokens(User user)
    {
        user.AssertNotNull();

        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name)
        };

        string accessToken = Generate(options.Value.SymmetricAccessKey,
        options.Value.AccessTokenExpirationMinutes,
        claims);
        string refreshToken = Generate(options.Value.SymmetricRefreshKey,
        options.Value.RefreshTokenExpirationMinutes);

        return new Tokens(accessToken, refreshToken);
    }

    private bool TryValidateRefreshToken(string? refreshToken, out ClaimsPrincipal? claimsPrincipal)
    {
        try
        {
            claimsPrincipal = JwtSecurityTokenHandler.ValidateToken(refreshToken, refreshTokenValidationParams, out SecurityToken _);
            return true;
        }
        catch (Exception)
        {
            claimsPrincipal = null;
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