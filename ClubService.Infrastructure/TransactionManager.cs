using ClubService.Domain.Repository.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ClubService.Infrastructure;

public class TransactionManager<TDbContext>(TDbContext dbContext)
    : IReadStoreTransactionManager, IEventStoreTransactionManager where TDbContext : DbContext
{
    private IDbContextTransaction? _transaction;
    
    public async Task BeginTransactionAsync()
    {
        _transaction = await dbContext.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
            return;
        }
        
        await _transaction.CommitAsync();
        await DisposeTransactionAsync();
    }
    
    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null)
        {
            return;
        }
        
        await _transaction.RollbackAsync();
        await DisposeTransactionAsync();
    }
    
    public void Dispose()
    {
        if (_transaction == null)
        {
            return;
        }
        
        _transaction.Dispose();
        _transaction = null;
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