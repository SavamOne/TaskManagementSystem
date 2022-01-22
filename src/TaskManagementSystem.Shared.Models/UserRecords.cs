namespace TaskManagementSystem.Shared.Models;

public record RefreshTokensRequest(string RefreshToken);

public record LoginRequest(string Email, string Password);

public record RegisterRequest(string Email, string Username, string Password);

public record RefreshTokensResponse(bool IsSuccess, Tokens? Tokens, string? ErrorDescription);

public record LoginResponse(bool IsSuccess, Tokens? Tokens, string? ErrorDescription)
    : RefreshTokensResponse(IsSuccess, Tokens, ErrorDescription);

public record RegisterResponse(bool IsSuccess, Tokens? Tokens, string? ErrorDescription) 
    : LoginResponse(IsSuccess, Tokens, ErrorDescription);