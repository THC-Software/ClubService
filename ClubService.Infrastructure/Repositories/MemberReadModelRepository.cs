using ClubService.Domain.Event;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;

namespace ClubService.Infrastructure.Repositories;

public class MemberReadModelRepository(ReadStoreDbContext readStoreDbContext) : IMemberReadModelRepository
{
    public async Task Add(MemberReadModel memberReadModel)
    {
        await readStoreDbContext.Members.AddAsync(memberReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }
}