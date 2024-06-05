using System.Data;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Repository.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ClubService.Infrastructure;

public class TransactionManager<TDbContext>(TDbContext dbContext)
    : IReadStoreTransactionManager, IEventStoreTransactionManager where TDbContext : DbContext
{
    private IDbContextTransaction? _transaction;
    
    public void Dispose()
    {
        if (_transaction == null)
        {
            return;
        }
        
        _transaction.Dispose();
        _transaction = null;
    }
    
    public async Task TransactionScope(Func<Task> transactionalFunction)
    {
        try
        {
            await BeginTransactionAsync();
            await transactionalFunction();
            await CommitTransactionAsync();
        }
        catch (DataException ex)
        {
            await RollbackTransactionAsync();
            throw new ConcurrencyException(ex.Message, ex);
        }
    }
    
    
    private async Task BeginTransactionAsync()
    {
        _transaction = await dbContext.Database.BeginTransactionAsync();
    }
    
    private async Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
            return;
        }
        
        await _transaction.CommitAsync();
        await DisposeTransactionAsync();
    }
    
    private async Task RollbackTransactionAsync()
    {
        if (_transaction == null)
        {
            return;
        }
        
        await _transaction.RollbackAsync();
        await DisposeTransactionAsync();
    }
    
    private async Task DisposeTransactionAsync()
    {
        if (_transaction == null)
        {
            return;
        }
        
        await _transaction.DisposeAsync();
        _transaction = null;
    }
}