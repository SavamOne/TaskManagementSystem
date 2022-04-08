using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Shared.Models.Requests;

/// <summary>
/// Запрос на подписку webpush уведомлений для устройства (https://datatracker.ietf.org/doc/html/draft-ietf-webpush-encryption-08)
/// </summary>
public class AddNotificationSubscribeRequest
{
	public AddNotificationSubscribeRequest(string url, string p256dh, string auth)
	{
		Url = url;
		P256dh = p256dh;
		Auth = auth;
	}

	/// <summary>
	/// Адрес Push сервиса.
	/// </summary>
	[Required]
	public string Url { get; }

	/// <summary>
	/// Открытый ключ.
	/// </summary>
	[Required]
	public string P256dh { get; }

	/// <summary>
	/// Auth secret.
	/// </summary>
	[Required]
	public string Auth { get; }
}

/// <summary>
/// Запрос на отписку webpush уведомлений устройства
/// </summary>
public class DeleteNotificationSubscribeRequest
{
	public DeleteNotificationSubscribeRequest(string url) 
	{
		Url = url;
	}

	/// <summary>
	/// Адрес Push сервиса.
	/// </summary>
	[Required]
	public string Url { get; }
}

/// <summary>
/// Публичный ключ сервера приложения.
/// </summary>
public class GetPublicKeyResponse
{
	public GetPublicKeyResponse(string publicKey) 
	{
		PublicKey = publicKey;
	}

	/// <summary>
	/// Значение ключа.
	/// </summary>
	[Required]
	public string PublicKey { get; }
}