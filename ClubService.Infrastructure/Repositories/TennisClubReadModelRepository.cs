using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure.Repositories;

public class TennisClubReadModelRepository(ReadStoreDbContext readStoreDbContext) : ITennisClubReadModelRepository
{
    public async Task Add(TennisClubReadModel tennisClubReadModel)
    {
        await readStoreDbContext.TennisClubs.AddAsync(tennisClubReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }
    
    public async Task Update()
    {
        await readStoreDbContext.SaveChangesAsync();
    }
    
    public async Task<TennisClubReadModel?> GetTennisClubById(Guid id)
    {
        return await readStoreDbContext.TennisClubs
            .Where(tennisClub => tennisClub.TennisClubId == new TennisClubId(id))
            .SingleOrDefaultAsync();
    }
}