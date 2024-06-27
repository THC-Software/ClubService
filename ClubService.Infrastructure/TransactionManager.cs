using System.Data;
using System.Transactions;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Infrastructure;

public class TransactionManager : ITransactionManager
{
    public async Task TransactionScope(Func<Task> transactionalFunction)
    {
        using var scope =
            new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            await transactionalFunction();

            // The Complete method commits the transaction. If an exception has been thrown,
            // Complete is not  called and the transaction is rolled back.
            scope.Complete();
        }
        catch (DataException ex)
        {
            throw new ConcurrencyException(ex.Message, ex);
        }
    }
}