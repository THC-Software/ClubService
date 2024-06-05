namespace ClubService.Domain.Repository.Transaction;

public interface ITransactionManager : IDisposable
{
    Task TransactionScope(Func<Task> transactionalFunction);
}