﻿using TaskManagementSystem.BusinessLogic.Dal.Converters;
using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories.Implementations;

public class UserRepository : Repository<DalUser>, IUserRepository
{
    public UserRepository(DatabaseConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        DalUser? dalUser = await FirstOrDefaultAsync(x => x.Id == id);
        
        return dalUser?.ToUser();
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        email.AssertNotNullOrWhiteSpace();
        
        DalUser? dalUser = await FirstOrDefaultAsync(x => x.Email == email);
        
        return dalUser?.ToUser();
    }

    public async Task UpdateAsync(User user)
    {
        user.AssertNotNull();
        
        await base.UpdateAsync(user.ToDalUser());
    }

    public async Task InsertAsync(User user)
    {
        user.AssertNotNull();

        await base.InsertAsync(user.ToDalUser());
    }
}