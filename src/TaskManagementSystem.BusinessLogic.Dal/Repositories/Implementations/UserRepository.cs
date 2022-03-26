using Dommel;
using TaskManagementSystem.BusinessLogic.Dal.Converters;
using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories.Implementations;

public class UserRepository : Repository<DalUser>, IUserRepository
{
	public UserRepository(DatabaseConnectionProvider connectionProvider)
		: base(connectionProvider) {}

	public async Task<User?> GetByIdAsync(Guid id)
	{
		DalUser? dalUser = await FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

		return dalUser?.ToUser();
	}

	public async Task<ISet<User>> GetByIdsAsync(ISet<Guid> ids)
	{
		ids.AssertNotNull();

		var dalUsers = await SelectAsync(x => ids.Contains(x.Id) && !x.IsDeleted);

		return dalUsers.Select(x => x.ToUser()).ToHashSet();
	}

	public async Task<User?> GetByEmailAsync(string email)
	{
		email.AssertNotNullOrWhiteSpace();

		//TODO: CaseSensitive
		DalUser? dalUser = await FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted);

		return dalUser?.ToUser();
	}
	public async Task<ISet<User>> GetByFilter(string filter, int limit)
	{
		filter.AssertNotNull();

		//TODO: CaseSensitive 
		var dalUsers = await GetConnection()
		   .FromAsync<DalUser>(sql =>
				sql.Select()
				   .Where(x => ( x.Name.StartsWith(filter) || x.Email.StartsWith(filter) ) && !x.IsDeleted)
				   .Page(1, limit));

		return dalUsers.Select(x => x.ToUser()).ToHashSet();
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