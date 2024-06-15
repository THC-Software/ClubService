using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure.Repositories;

public class MemberReadModelRepository(ReadStoreDbContext readStoreDbContext) : IMemberReadModelRepository
{
    public async Task Add(MemberReadModel memberReadModel)
    {
        await readStoreDbContext.Members.AddAsync(memberReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }
    
    public async Task Update()
    {
        await readStoreDbContext.SaveChangesAsync();
    }
    
    public async Task Delete(MemberReadModel memberReadModel)
    {
        readStoreDbContext.Members.Remove(memberReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }
    
    public async Task DeleteMembersByTennisClubId(Guid tennisClubId)
    {
        var membersToDelete = await readStoreDbContext.Members
            .Where(memberReadModel => memberReadModel.TennisClubId == new TennisClubId(tennisClubId))
            .ToListAsync();
        
        readStoreDbContext.Members.RemoveRange(membersToDelete);
        
        await readStoreDbContext.SaveChangesAsync();
    }
    
    public async Task<MemberReadModel?> GetMemberById(Guid id)
    {
        return await readStoreDbContext.Members
            .Where(memberReadModel => memberReadModel.MemberId == new MemberId(id))
            .SingleOrDefaultAsync();
    }
    
    public async Task<List<MemberReadModel>> GetMembersByTennisClubId(Guid tennisClubId)
    {
        return await readStoreDbContext.Members
            .Where(memberReadModel => memberReadModel.TennisClubId == new TennisClubId(tennisClubId))
            .ToListAsync();
    }
    
    public async Task<MemberReadModel?> GetMemberByTennisClubIdAndUsername(Guid tennisClubId, string username)
    {
        return await readStoreDbContext.Members
            .Where(member => member.TennisClubId == new TennisClubId(tennisClubId) && member.Email == username)
            .SingleOrDefaultAsync();
    }
}