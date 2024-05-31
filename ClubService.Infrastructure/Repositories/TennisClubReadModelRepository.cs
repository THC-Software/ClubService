using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;

namespace ClubService.Infrastructure.Repositories;

public class TennisClubReadModelRepository(ReadStoreDbContext readStoreDbContext) : ITennisClubReadModelRepository
{
    public async Task Add(TennisClubReadModel tennisClubReadModel)
    {
        await readStoreDbContext.TennisClubs.AddAsync(tennisClubReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }
}