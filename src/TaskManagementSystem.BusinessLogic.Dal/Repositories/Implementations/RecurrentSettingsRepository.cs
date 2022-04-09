using System.Data;
using TaskManagementSystem.BusinessLogic.Dal.Converters;
using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Dal;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories.Implementations;

public class RecurrentSettingsRepository : Repository<DalRecurrentSetting>, IRecurrentSettingsRepository
{
	public RecurrentSettingsRepository(DatabaseConnectionProvider connectionProvider)
		: base(connectionProvider) {}
	
	public async Task<RecurrentEventSettings?> GetForEvent(Guid eventId)
	{
		var settings = await SelectAsync(x => x.EventId == eventId);

		return settings.ToRecurrentSettings();
	}
	
	public async Task<ICollection<RecurrentEventSettings>> GetForEvents(ISet<Guid> eventIds)
	{
		var settings = await SelectAsync(x => eventIds.Contains(x.EventId));

		return settings.GroupBy(x => x.EventId).Select(x => x.ToRecurrentSettings()).ToList()!;
	}
	
	public async Task SaveForEvent(RecurrentEventSettings settings)
	{
		using IDbTransaction transaction = GetConnection().BeginTransaction();
		
		await DeleteMultipleAsync(x => x.EventId == settings.EventId);
		await InsertAllAsync(settings.ToDalSettings());
		
		transaction.Commit();
	}
}