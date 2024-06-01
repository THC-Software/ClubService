namespace ClubService.Domain.Repository;

public interface IReadStoreTransactionManager : IDisposable
{
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}