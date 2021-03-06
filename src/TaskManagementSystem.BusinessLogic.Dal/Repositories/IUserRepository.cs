using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories;

public interface IUserRepository
{
	Task<User?> GetByIdAsync(Guid id);

	Task<ISet<User>> GetByIdsAsync(ISet<Guid> ids);

	Task<User?> GetByEmailAsync(string email);

	Task<ISet<User>> GetByFilter(string filter, int limit);

	Task UpdateAsync(User user);

	Task InsertAsync(User user);
}