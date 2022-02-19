using TaskManagementSystem.BusinessLogic.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByEmailAsync(string email);

    Task UpdateAsync(User user);

    Task InsertAsync(User user);
}