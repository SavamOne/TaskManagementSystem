using System.Data;
using System.Linq.Expressions;
using Dommel;

namespace TaskManagementSystem.Shared.Dal;

public class Repository<TModel> where TModel : class
{
    private readonly DatabaseConnectionProvider connectionProvider;

    public Repository(DatabaseConnectionProvider connectionProvider) 
    {
        this.connectionProvider = connectionProvider;
    }

    protected IDbConnection GetConnection()
    {
        return connectionProvider.GetConnection();
    }

    protected async Task<IEnumerable<TModel>> SelectAsync(Expression<Func<TModel, bool>> expression)
    {
        return await GetConnection().SelectAsync(expression);
    }

    protected async Task InsertAsync(TModel model)
    {
        await GetConnection().InsertAsync(model);
    }
    
    protected async Task InsertAllAsync(IEnumerable<TModel> model)
    {
        await GetConnection().InsertAllAsync(model);
    }
    
    protected async Task UpdateAsync(TModel model)
    {
        await GetConnection().UpdateAsync(model);
    }

    protected async Task<TModel?> FirstOrDefaultAsync(Expression<Func<TModel, bool>> expression)
    {
        return await GetConnection().FirstOrDefaultAsync(expression);
    }

    protected async Task<int> DeleteMultipleAsync(Expression<Func<TModel, bool>> expression)
    {
        return await GetConnection().DeleteMultipleAsync(expression);
    }

}