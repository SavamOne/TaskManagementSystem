using Dapper;
using TaskManagementSystem.BusinessLogic.Dal.Converters;
using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories.Implementations;

public class CalendarRepository : Repository<DalCalendar>, ICalendarRepository
{
	public CalendarRepository(DatabaseConnectionProvider connectionProvider)
		: base(connectionProvider) {}

	public async Task<Calendar?> GetByIdAsync(Guid id)
	{
		DalCalendar? dalCalendar = await FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

		return dalCalendar?.ToCalendar();
	}

	public async Task<Calendar?> GetByNameAsync(string name)
	{
		name.AssertNotNullOrWhiteSpace();

		//TODO: CaseSensitive
		DalCalendar? dalCalendar = await FirstOrDefaultAsync(x => x.Name == name && !x.IsDeleted);

		return dalCalendar?.ToCalendar();
	}

	public async Task<ICollection<Calendar>> GetByIdsAsync(ISet<Guid> ids)
	{
		var dalCalendars = await SelectAsync(x => ids.Contains(x.Id) && !x.IsDeleted);

		return dalCalendars.Select(x => x.ToCalendar()).ToList();
	}

	public async Task<ICollection<Calendar>> GetByUserId(Guid userId)
	{
		const string getSql = "SELECT C.* FROM Calendar_Participant CP "
							+ "INNER JOIN Calendar C ON CP.Calendar_Id = C.id "
							+ "WHERE Cp.User_Id = @UserId AND C.Is_Deleted = FALSE AND CP.Is_Deleted = FALSE; ";

		var dalCalendars = await GetConnection()
		   .QueryAsync<DalCalendar>(getSql,
				new
				{
					userId
				});

		return dalCalendars.Select(x => x.ToCalendar()).ToList();
	}

	public async Task InsertAsync(Calendar calendar)
	{
		calendar.AssertNotNull();

		await InsertAsync(calendar.ToDalCalendar());
	}

	public async Task UpdateAsync(Calendar calendar)
	{
		calendar.AssertNotNull();

		await UpdateAsync(calendar.ToDalCalendar());
	}
}