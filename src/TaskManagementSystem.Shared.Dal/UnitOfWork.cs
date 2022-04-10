using System.Data;

namespace TaskManagementSystem.Shared.Dal;

public class UnitOfWork : IUnitOfWork
{
	private readonly DatabaseConnectionProvider connectionProvider;

	private IDbTransaction? transaction;

	public UnitOfWork(DatabaseConnectionProvider connectionProvider)
	{
		this.connectionProvider = connectionProvider;
	}

	public void BeginTransaction()
	{
		transaction ??= connectionProvider.GetConnection().BeginTransaction();
	}

	public void CommitTransaction()
	{
		transaction?.Commit();
		Dispose();
	}

	public void RollbackTransaction()
	{
		transaction?.Rollback();
		Dispose();
	}

	public void Dispose()
	{
		transaction?.Dispose();
		transaction = null;
	}
}