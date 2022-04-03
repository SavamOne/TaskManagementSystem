namespace TaskManagementSystem.Shared.Models;

public record AddNotificationSubscribeRequest(string Url, string P256dh, string Auth);

public record DeleteNotificationSubscribeRequest(string Url);

public record GetPublicKeyResponse(string PublicKey);