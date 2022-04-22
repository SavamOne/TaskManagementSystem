using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSystem.BusinessLogic.Dal.Repositories;
using TaskManagementSystem.BusinessLogic.Models.Exceptions;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Server.Dal.Repositories;
using TaskManagementSystem.Server.Exceptions;
using TaskManagementSystem.Server.Options;
using TaskManagementSystem.Server.Resources;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Server.Services.Implementations;

public class TokenService : ITokenService
{
	private static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler = new();

	private readonly IOptions<JwtOptions> options;
	private readonly TokenValidationParameters refreshTokenValidationParams;
	private readonly IRefreshTokenRepository tokenRepository;
	private readonly IUserRepository userRepository;

	public TokenService(IOptions<JwtOptions> options, IUserRepository userRepository, IRefreshTokenRepository tokenRepository)
	{
		this.options = options;
		this.userRepository = userRepository;
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

		var userId = await tokenRepository.GetUserIdFromTokenAsync(refreshToken);

		if (!userId.HasValue)
		{
			throw new ServerException(LocalizedResources.TokenService_RefreshTokenDoesNotExistsOrCancelled);
		}

		await ValidateRefreshToken(refreshToken);

		User user = ( await userRepository.GetByIdAsync(userId.Value) )!;

		Tokens tokens = GenerateTokens(user);

		try
		{
			await tokenRepository.UpdateForUserAsync(userId.Value, refreshToken, tokens.RefreshToken);
			return tokens;
		}
		catch
		{
			throw new ServerException("Ошибка при обновлении refresh токена.");
		}
	}

	public async Task RemoveTokenAsync(string refreshToken)
	{
		refreshToken.AssertNotNullOrWhiteSpace();

		await tokenRepository.RemoveTokenAsync(refreshToken);
	}

	public Guid GetUserIdFromClaims(ClaimsPrincipal principal)
	{
		principal.AssertNotNull();

		string? idClaim = principal.FindFirstValue(JwtRegisteredClaimNames.NameId);

		if (!Guid.TryParse(idClaim, out Guid id))
		{
			throw new ApplicationException($"Claim '{JwtRegisteredClaimNames.NameId}' does not exists or is incorrect GUID");
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

	private async Task ValidateRefreshToken(string refreshToken)
	{
		try
		{
			JwtSecurityTokenHandler.ValidateToken(refreshToken, refreshTokenValidationParams, out SecurityToken _);
		}
		catch (SecurityTokenExpiredException)
		{
			await tokenRepository.RemoveTokenAsync(refreshToken);
			throw new ServerException(LocalizedResources.TokenService_RefreshTokenIsOutdated);
		}
		catch (Exception ex)
		{
			await tokenRepository.RemoveTokenAsync(refreshToken);
			throw new ApplicationException("Incorrect token in database", ex);
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
			nowDate.AddSeconds(expirationMinutes),
			credentials);
		return JwtSecurityTokenHandler.WriteToken(securityToken);
	}
}