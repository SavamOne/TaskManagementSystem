using Dapper;
using Dommel;
using TaskManagementSystem.BusinessLogic.Dal.Converters;
using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories.Implementations;

public class CalendarParticipantRepository : Repository<DalCalendarParticipant>, ICalendarParticipantRepository
{
	public CalendarParticipantRepository(DatabaseConnectionProvider connectionProvider)
		: base(connectionProvider) {}

	public async Task<ICollection<CalendarParticipant>> GetByCalendarIdAsync(Guid calendarId)
	{
		var dalParticipants = await GetConnection().SelectAsync<DalCalendarParticipant, DalUser, DalCalendarParticipant>(
			x => x.CalendarId == calendarId && !x.IsDeleted, StandardMap);

		return dalParticipants.Select(x => x.ToCalendarParticipant()).ToList();
	}
	public async Task<CalendarParticipant?> GetByUserAndCalendarId(Guid userId, Guid calendarId)
	{
		DalCalendarParticipant? dalParticipant = await GetConnection().FirstOrDefaultAsync<DalCalendarParticipant, DalUser, DalCalendarParticipant>(
			x => x.UserId == userId && x.CalendarId == calendarId && !x.IsDeleted, StandardMap);

		return dalParticipant?.ToCalendarParticipant();
	}

	public async Task<ICollection<CalendarParticipant>> GetByIdsAsync(ISet<Guid> ids)
	{
		ids.AssertNotNull();

		var dalParticipants =await GetConnection().SelectAsync<DalCalendarParticipant, DalUser, DalCalendarParticipant>(
			x => ids.Contains(x.Id) && !x.IsDeleted, StandardMap);

		return dalParticipants.Select(x => x.ToCalendarParticipant()).ToList();
	}

	public async Task InsertAsync(CalendarParticipant calendar)
	{
		calendar.AssertNotNull();
		await InsertAsync(calendar.ToDalCalendarParticipant());
	}

	public async Task InsertAllAsync(ISet<CalendarParticipant> calendarParticipants)
	{
		const string insertSql = "INSERT INTO Calendar_Participant (Id, Calendar_Id, User_Id, Role, Join_Date) "
							   + "VALUES (@Id, @CalendarId, @UserId, @Role, @JoinDate) "
							   + "ON CONFLICT (Calendar_Id, User_Id) "
							   + "DO UPDATE SET Role = @Role, Join_Date = @JoinDate, Is_Deleted = FALSE;";

		calendarParticipants.AssertNotNull();
		await GetConnection().ExecuteAsync(insertSql, calendarParticipants.Select(x => x.ToDalCalendarParticipant()).ToArray());
	}

	public async Task UpdateAllAsync(ICollection<CalendarParticipant> calendarParticipants)
	{
		calendarParticipants.AssertNotNull();
		foreach (CalendarParticipant calendarParticipant in calendarParticipants)
		{
			await UpdateAsync(calendarParticipant.ToDalCalendarParticipant());
		}
	}

	public async Task DeleteByIdsAsync(ISet<Guid> calendarParticipantsIds)
	{
		const string deleteSql = "UPDATE calendar_participant SET is_deleted = TRUE WHERE id = ANY(@Ids);";

		calendarParticipantsIds.AssertNotNull();

		await GetConnection()
		   .ExecuteScalarAsync(deleteSql,
				new
				{
					Ids = calendarParticipantsIds.ToList()
				});
	}
	public async Task<ICollection<CalendarParticipant>> GetByFilter(Guid calendarId, string filter, int limit)
	{
		const string getSql = "SELECT * FROM calendar_participant cp "
							+ "INNER JOIN \"user\" u on u.id = cp.user_id "
							+ "WHERE cp.calendar_id = @CalendarId "
							+ "and (upper(u.name) like concat(upper(@Filter), '%') "
							+ "or upper(u.email) like concat(upper(@Filter), '%')) "
							+ "and cp.is_deleted = false and u.is_deleted = false " 
							 + "limit @Limit";

		var dalParticipants = await GetConnection()
		   .QueryAsync<DalCalendarParticipant, DalUser, DalCalendarParticipant>(getSql,
				StandardMap,
				new
				{
					Filter = filter,
					CalendarId = calendarId,
					Limit = limit
				});

		return dalParticipants.Select(x => x.ToCalendarParticipant()).ToList();
	}

	private static DalCalendarParticipant StandardMap(DalCalendarParticipant participant, DalUser user)
	{
		participant.User = user;
		return participant;
	}
}