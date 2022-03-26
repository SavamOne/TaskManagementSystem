using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories;

public interface ICalendarRepository
{
    Task<Calendar?> GetByIdAsync(Guid id);

    Task<Calendar?> GetByNameAsync(string name);

    Task<ICollection<Calendar>> GetByUserId(Guid userId);

    Task InsertAsync(Calendar calendar);

    Task UpdateAsync(Calendar calendar);
}