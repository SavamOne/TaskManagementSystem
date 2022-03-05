namespace TaskManagementSystem.Shared.Models;

public record RefreshTokensRequest(string RefreshToken);

public record LoginRequest(string Email, string Password);

public record RegisterRequest(string Email, string Name, string Password);

public record ChangePasswordRequest(string OldPassword, string NewPassword);

public record ChangeUserInfoRequest(string Name, string Email);

public record GetUserInfoByIdRequest(Guid UserId);

public record GetUserInfosByFilterRequest(string Filter);