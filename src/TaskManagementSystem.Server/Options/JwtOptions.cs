using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TaskManagementSystem.Server.Options;

public class JwtOptions
{
	private readonly Encoding encoding = Encoding.ASCII;
	private SymmetricSecurityKey? symmetricAccessKey;
	private SymmetricSecurityKey? symmetricRefreshKey;

	public string? Issuer { get; set; }

	public string? Audience { get; set; }

	public string? AccessTokenSecretKey { get; set; }

	public string? RefreshTokenSecretKey { get; set; }

	public int AccessTokenExpirationMinutes { get; set; }

	public int RefreshTokenExpirationMinutes { get; set; }

	public SymmetricSecurityKey SymmetricAccessKey => symmetricAccessKey ??= new SymmetricSecurityKey(encoding.GetBytes(AccessTokenSecretKey!));

	public SymmetricSecurityKey SymmetricRefreshKey => symmetricRefreshKey ??= new SymmetricSecurityKey(encoding.GetBytes(RefreshTokenSecretKey!));
}