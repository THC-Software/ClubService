using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;

namespace ClubService.Infrastructure.Repositories;

public class AdminReadModelRepository(ReadStoreDbContext readStoreDbContext) : IAdminReadModelRepository
{
    public async Task Add(AdminReadModel adminReadModel)
    {
        await readStoreDbContext.Admins.AddAsync(adminReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }
    
    public async Task Delete(AdminReadModel adminReadModel)
    {
        readStoreDbContext.Admins.Remove(adminReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }
}