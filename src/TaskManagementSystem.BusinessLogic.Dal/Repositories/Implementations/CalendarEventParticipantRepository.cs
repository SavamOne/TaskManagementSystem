using Dapper;
using TaskManagementSystem.BusinessLogic.Dal.Converters;
using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories.Implementations;

public class CalendarEventParticipantRepository : Repository<DalEventParticipant>
{
	public CalendarEventParticipantRepository(DatabaseConnectionProvider connectionProvider)
		: base(connectionProvider) {}

	public async Task InsertAsync(CalendarEventParticipant participant)
	{
		await InsertAsync(participant.ToDalEventParticipant());
	}

	public async Task InsertAllAsync(ICollection<CalendarEventParticipant> eventParticipants)
	{
		const string insertSql = "INSERT INTO Event_Participant (Id, Event_Id, calendar_participant_id, Role, Status_type) "
							   + "VALUES (@Id, @EventId, @CalendarParticipantId, @Role, 0) "
							   + "ON CONFLICT (Event_Id, calendar_participant_id) "
							   + "DO UPDATE SET Role = @Role, Is_Deleted = FALSE;";

		eventParticipants.AssertNotNull();
		await GetConnection().ExecuteAsync(insertSql, eventParticipants.Select(x => x.ToDalEventParticipant()).ToList());
	}

	public async Task<bool> ContainsCalendarParticipantInEvent(Guid calendarParticipant, Guid eventId)
	{
		return await AnyAsync(x => x.CalendarParticipantId == calendarParticipant && x.EventId == eventId);
	}

	public async Task<CalendarEventParticipant?> GetByUserAndEventId(Guid userId, Guid eventId)
	{
		const string getSql = "SELECT * FROM event_participant ep "
							+ "INNER JOIN calendar_participant cp on cp.id = ep.calendar_participant_id "
							+ "INNER JOIN \"user\" u on cp.user_id = u.id "
							+ "WHERE cp.user_id = @UserId AND ep.event_id = @EventId "
							+ "AND ep.is_deleted = false and cp.is_deleted = false AND u.is_deleted = false";
		

		DalEventParticipant? participant = ( await GetConnection()
		   .QueryAsync<DalEventParticipant, DalCalendarParticipant, DalUser, DalEventParticipant>(getSql,
				(eventParticipant, calendarParticipant, user) =>
				{
					calendarParticipant.User = user;
					eventParticipant.CalendarParticipant = calendarParticipant;
					return eventParticipant;
				},
				new
				{
					userId,
					eventId
				}) ).FirstOrDefault();

		return participant?.ToEventParticipant();
	}

	public async Task<ICollection<CalendarEventParticipant>> GetByEventId(Guid eventId)
	{
		const string getSql = "SELECT * FROM event_participant ep "
							+ "INNER JOIN calendar_participant cp on cp.id = ep.calendar_participant_id "
							+ "INNER JOIN \"user\" u on cp.user_id = u.id "
							+ "WHERE ep.event_id = @EventId "
							+ "AND ep.is_deleted = false and cp.is_deleted = false AND u.is_deleted = false";

		var dalParticipants = await GetConnection()
		   .QueryAsync<DalEventParticipant, DalCalendarParticipant, DalUser, DalEventParticipant>(getSql,
				(eventParticipant, calendarParticipant, user) =>
				{
					calendarParticipant.User = user;
					eventParticipant.CalendarParticipant = calendarParticipant;
					return eventParticipant;
				},
				new
				{
					eventId
				});

		return dalParticipants.Select(x => x.ToEventParticipant()).ToList();
	}

	public async Task UpdateAllAsync(ICollection<CalendarEventParticipant> eventParticipants)
	{
		eventParticipants.AssertNotNull();
		foreach (CalendarEventParticipant calendarParticipant in eventParticipants)
		{
			await UpdateAsync(calendarParticipant.ToDalEventParticipant());
		}
	}
	
	public async Task DeleteByIdsAsync(ISet<Guid> eventParticipantsIds)
	{
		const string deleteSql = "UPDATE event_participant SET is_deleted = TRUE WHERE id = ANY(@Ids);";

		eventParticipantsIds.AssertNotNull();

		await GetConnection()
		   .ExecuteScalarAsync(deleteSql,
				new
				{
					Ids = eventParticipantsIds.ToList()
				});
	}
}