using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure.Repositories;

public class ProcessedEventRepository(ReadStoreDbContext readStoreDbContext) : IProcessedEventRepository
{
    public async Task Add(ProcessedEvent processedEvent)
    {
        await readStoreDbContext.ProcessedEvents.AddAsync(processedEvent);
        await readStoreDbContext.SaveChangesAsync();
    }
}