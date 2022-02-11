namespace TaskManagementSystem.Shared.Models;

public record RefreshTokensRequest(string RefreshToken);

public record LoginRequest(string Email, string Password);

public record RegisterRequest(string Email, string Name, string Password);

public record RefreshTokensResponse(bool IsSuccess, Tokens? Tokens, string? ErrorDescription);

public record LoginResponse(bool IsSuccess, Tokens? Tokens, string? ErrorDescription)
    : RefreshTokensResponse(IsSuccess, Tokens, ErrorDescription);

public record RegisterResponse(bool IsSuccess, Tokens? Tokens, string? ErrorDescription)
    : LoginResponse(IsSuccess, Tokens, ErrorDescription);
    
public record GetUserInfoResponse(bool IsSuccess, UserInfo? UserInfo, string? ErrorDescription);

public record ChangePasswordRequest(string OldPassword, string NewPassword);

public record ChangeUserInfoRequest(string Name, string Email);

public record ChangePasswordResponse(bool IsSuccess, UserInfo? UserInfo, string? ErrorDescription) 
    : GetUserInfoResponse(IsSuccess, UserInfo, ErrorDescription);
    
public record ChangeUserInfoResponse(bool IsSuccess, UserInfo? UserInfo, string? ErrorDescription) 
    : GetUserInfoResponse(IsSuccess, UserInfo, ErrorDescription);