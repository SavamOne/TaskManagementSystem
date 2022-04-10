using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories;

public interface IRecurrentSettingsRepository
{
	Task<RecurrentEventSettings?> GetForEvent(Guid eventId);
	
	Task DeleteForEvent(Guid eventId);
	
	Task SaveForEvent(RecurrentEventSettings settings);

	Task<ICollection<RecurrentEventSettings>> GetForEvents(ISet<Guid> eventIds);
}