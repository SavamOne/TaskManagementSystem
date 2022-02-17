using System.Data;
using TaskManagementSystem.Server.Dal.DalModels;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Server.Dal.Repositories.Implementations;

public class RefreshTokenRepository : Repository<DalUserToken>, IRefreshTokenRepository {

    public RefreshTokenRepository(DatabaseConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
    }

    public async Task InsertForUserAsync(Guid userId, string refreshToken)
    {
        refreshToken.AssertNotNullOrWhiteSpace();
        
        DalUserToken entry = new()
        {
            UserId = userId,
            RefreshToken = refreshToken
        };

        await base.InsertAsync(entry);
    }

    public async Task UpdateForUserAsync(Guid userId, string oldRefreshToken, string newRefreshToken)
    {
        oldRefreshToken.AssertNotNullOrWhiteSpace();
        newRefreshToken.AssertNotNullOrWhiteSpace();

        using IDbTransaction transaction = GetConnection().BeginTransaction();
        await DeleteMultipleAsync(x => x.RefreshToken == newRefreshToken);
        await InsertForUserAsync(userId, newRefreshToken);
        transaction.Commit();
    }

    public async Task ClearForUserAsync(Guid userId)
    {
        await DeleteMultipleAsync(x => x.UserId == userId);
    }
}