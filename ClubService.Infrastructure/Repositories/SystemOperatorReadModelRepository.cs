using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure.Repositories;

public class SystemOperatorReadModelRepository(ReadStoreDbContext readStoreDbContext)
    : ISystemOperatorReadModelRepository
{
    public async Task Add(SystemOperatorReadModel systemOperatorReadModel)
    {
        readStoreDbContext.SystemOperators.Add(systemOperatorReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }

    public async Task<SystemOperatorReadModel?> GetSystemOperatorByUsername(string username)
    {
        return await readStoreDbContext.SystemOperators
            .Where(systemOperatorReadModel => systemOperatorReadModel.Username == username)
            .SingleOrDefaultAsync();
    }
}