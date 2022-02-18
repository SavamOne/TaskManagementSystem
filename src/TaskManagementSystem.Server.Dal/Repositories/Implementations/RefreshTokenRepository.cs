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

        await InsertAsync(entry);
    }

    public async Task UpdateForUserAsync(Guid userId, string oldRefreshToken, string newRefreshToken)
    {
        oldRefreshToken.AssertNotNullOrWhiteSpace();
        newRefreshToken.AssertNotNullOrWhiteSpace();

        using IDbTransaction transaction = GetConnection().BeginTransaction();
        await RemoveTokenAsync(oldRefreshToken);
        await InsertForUserAsync(userId, newRefreshToken);
        transaction.Commit();
    }
    
    public async Task<Guid?> GetUserIdFromTokenAsync(string refreshToken)
    {
        refreshToken.AssertNotNullOrWhiteSpace();

        DalUserToken? userToken = await FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);

        return userToken?.UserId;
    }
    
    public async Task RemoveTokenAsync(string refreshToken)
    {
        refreshToken.AssertNotNullOrWhiteSpace();

        await DeleteMultipleAsync(x => x.RefreshToken == refreshToken);
    }

    public async Task ClearForUserAsync(Guid userId)
    {
        await DeleteMultipleAsync(x => x.UserId == userId);
    }
}