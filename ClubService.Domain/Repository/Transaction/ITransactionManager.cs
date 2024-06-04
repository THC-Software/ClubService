namespace ClubService.Domain.Repository.Transaction;

public interface ITransactionManager : IDisposable
{
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}