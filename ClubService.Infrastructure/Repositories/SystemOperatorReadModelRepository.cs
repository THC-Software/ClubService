using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;

namespace ClubService.Infrastructure.Repositories;

public class SystemOperatorReadModelRepository(ReadStoreDbContext readStoreDbContext)
    : ISystemOperatorReadModelRepository
{
    public async Task Add(SystemOperatorReadModel systemOperatorReadModel)
    {
        readStoreDbContext.SystemOperators.Add(systemOperatorReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }
}