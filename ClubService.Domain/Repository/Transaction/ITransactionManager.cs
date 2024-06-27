namespace ClubService.Domain.Repository.Transaction;

public interface ITransactionManager
{
    Task TransactionScope(Func<Task> transactionalFunction);
}