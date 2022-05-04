using Dapper;
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
		const string filterSql = "SELECT * FROM \"user\" u "
							   + "WHERE (upper(u.name) like concat(upper(@Filter), '%') "
							   + "OR upper(u.email) like concat(upper(@Filter), '%')) "
							   + "and u.is_deleted = false "
							   + "limit @Limit";
		filter.AssertNotNull();

		//TODO: CaseSensitive 
		var dalUsers = await GetConnection()
		   .QueryAsync<DalUser>(filterSql,
				new
				{
					Filter = filter,
					Limit = limit
				});

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