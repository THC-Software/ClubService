using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;

namespace ClubService.Infrastructure.Repositories;

public class ProcessedEventRepository(ReadStoreDbContext readStoreDbContext) : IProcessedEventRepository
{
    public Task Add(ProcessedEvent processedEvent)
    {
        throw new NotImplementedException();
    }
    
    public Task<List<ProcessedEvent>> GetAllProcessedEvents()
    {
        throw new NotImplementedException();
    }
}