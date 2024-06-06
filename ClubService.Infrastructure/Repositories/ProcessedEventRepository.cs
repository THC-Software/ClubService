using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;

namespace ClubService.Infrastructure.Repositories;

public class ProcessedEventRepository(ReadStoreDbContext readStoreDbContext) : IProcessedEventRepository
{
    public Task Add(Guid id)
    {
        throw new NotImplementedException();
    }
    
    public Task<List<Guid>> GetAllProcessedEvents()
    {
        throw new NotImplementedException();
    }
}