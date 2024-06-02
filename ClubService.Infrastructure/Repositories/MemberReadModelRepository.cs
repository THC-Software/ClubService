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
    
    public async Task<MemberReadModel?> GetMemberById(Guid id)
    {
        return await readStoreDbContext.Members
            .Where(memberReadModel => memberReadModel.MemberId == new MemberId(id))
            .SingleOrDefaultAsync();
    }
}