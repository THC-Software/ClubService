using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure.Repositories;

public class AdminReadModelRepository(ReadStoreDbContext readStoreDbContext) : IAdminReadModelRepository
{
    public async Task Add(AdminReadModel adminReadModel)
    {
        await readStoreDbContext.Admins.AddAsync(adminReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }
    
    public async Task Update()
    {
        await readStoreDbContext.SaveChangesAsync();
    }
    
    public async Task Delete(AdminReadModel adminReadModel)
    {
        readStoreDbContext.Admins.Remove(adminReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }
    
    public async Task DeleteAdminsByTennisClubId(Guid tennisClubId)
    {
        var adminsToDelete = await readStoreDbContext.Admins
            .Where(admin => admin.TennisClubId.Id == tennisClubId)
            .ToListAsync();
        
        readStoreDbContext.Admins.RemoveRange(adminsToDelete);
        
        await readStoreDbContext.SaveChangesAsync();
    }
    
    public async Task<AdminReadModel?> GetAdminById(Guid id)
    {
        return await readStoreDbContext.Admins
            .Where(admin => admin.AdminId == new AdminId(id))
            .SingleOrDefaultAsync();
    }
    
    public async Task<List<AdminReadModel>> GetAdminsByTennisClubId(Guid tennisClubId)
    {
        return await readStoreDbContext.Admins
            .Where(admin => admin.TennisClubId == new TennisClubId(tennisClubId))
            .ToListAsync();
    }
    
    public async Task<AdminReadModel?> GetAdminByTennisClubIdAndUsername(Guid tennisClubId, string username)
    {
        return await readStoreDbContext.Admins
            .Where(admin => admin.TennisClubId == new TennisClubId(tennisClubId) && admin.Username == username)
            .SingleOrDefaultAsync();
    }
}