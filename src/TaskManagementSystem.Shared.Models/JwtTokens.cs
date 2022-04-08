using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

/// <summary>
/// Токены.
/// </summary>
public class Tokens
{
	public Tokens(string accessToken, string refreshToken)
	{
		AccessToken = accessToken.AssertNotNullOrWhiteSpace();
		RefreshToken = refreshToken.AssertNotNullOrWhiteSpace();
	}

	/// <summary>
	/// Access токен.
	/// </summary>
	[Required]
	public string AccessToken { get; }

	/// <summary>
	/// Refresh токен.
	/// </summary>
	[Required]
	public string RefreshToken { get; }
}