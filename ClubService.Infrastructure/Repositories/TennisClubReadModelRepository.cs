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
    
    public async Task Delete(TennisClubReadModel tennisClub)
    {
        readStoreDbContext.TennisClubs.Remove(tennisClub);
        await readStoreDbContext.SaveChangesAsync();
    }
    
    public async Task<List<TennisClubReadModel>> GetAllTennisClubs(int pageSize = 0, int pageNumber = 1)
    {
        IQueryable<TennisClubReadModel> tennisClubs = readStoreDbContext.TennisClubs;
        
        if (pageSize > 0)
        {
            tennisClubs = tennisClubs.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
        }
        
        return await tennisClubs.ToListAsync();
    }
}