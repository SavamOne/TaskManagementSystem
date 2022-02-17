namespace TaskManagementSystem.Server.Dal.Repositories;

public interface IRefreshTokenRepository
{
    Task InsertForUserAsync(Guid userId, string refreshToken);

    Task UpdateForUserAsync(Guid userId, string oldRefreshToken, string newRefreshToken);

    Task ClearForUserAsync(Guid userId);
}