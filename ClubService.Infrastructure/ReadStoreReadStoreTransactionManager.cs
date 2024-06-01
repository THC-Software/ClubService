using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace ClubService.Infrastructure;

public class ReadStoreReadStoreTransactionManager(ReadStoreDbContext readStoreDbContext) : IReadStoreTransactionManager
{
    private IDbContextTransaction? _transaction;
    
    public async Task BeginTransactionAsync()
    {
        _transaction = await readStoreDbContext.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await DisposeTransactionAsync();
        }
    }
    
    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await DisposeTransactionAsync();
        }
    }
    
    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    public void Dispose()
    {
        _transaction?.Dispose();
        _transaction = null;
    }
}