namespace TaskManagementSystem.Shared.Dal;

public interface IUnitOfWork : IDisposable
{
    void BeginTransaction();
    
    void CommitTransaction();
    
    void RollbackTransaction();
}