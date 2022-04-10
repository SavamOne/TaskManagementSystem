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
	
	public async Task DeleteForEvent(Guid eventId)
	{
		await DeleteMultipleAsync(x => x.EventId == eventId);
	}

	public async Task<ICollection<RecurrentEventSettings>> GetForEvents(ISet<Guid> eventIds)
	{
		var settings = await SelectAsync(x => eventIds.Contains(x.EventId));

		return settings.GroupBy(x => x.EventId).Select(x => x.ToRecurrentSettings()).ToList()!;
	}
	
	public async Task InsertForEvent(RecurrentEventSettings settings)
	{
		await InsertAllAsync(settings.ToDalSettings());
	}
}