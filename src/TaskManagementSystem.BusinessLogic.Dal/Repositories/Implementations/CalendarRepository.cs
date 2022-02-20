using Dapper;
using Dommel;
using TaskManagementSystem.BusinessLogic.Dal.Converters;
using TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Dal.Repositories.Implementations;

public class CalendarRepository : Repository<DalCalendar>, ICalendarRepository
{
    public CalendarRepository(DatabaseConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
        
    }

    public async Task<Calendar?> GetByIdAsync(Guid id)
    {
        DalCalendar? dalCalendar = await FirstOrDefaultAsync(x => x.Id == id);

        return dalCalendar?.ToCalendar();
    }

    public async Task<Calendar?> GetByNameAsync(string name)
    {
        name.AssertNotNullOrWhiteSpace();
        
        DalCalendar? dalCalendar = await FirstOrDefaultAsync(x => x.Name == name);
        
        return dalCalendar?.ToCalendar();
    }

    public async Task<ISet<Calendar>> GetByUserId(Guid userId)
    {
        var dalCalendars = await GetConnection().QueryAsync<DalCalendar>(
        "SELECT c.* FROM calendar_participant cp inner join calendar c on cp.calendar_id = c.id where cp.user_id == @userId", 
        new
        {
            userId
        });

        return dalCalendars.Select(x => x.ToCalendar()).ToHashSet();
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