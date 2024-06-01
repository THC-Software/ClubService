using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure.Repositories;

public class SubscriptionTierReadModelRepository(ReadStoreDbContext readStoreDbContext)
    : ISubscriptionTierReadModelRepository
{
    public async Task Add(SubscriptionTierReadModel subscriptionTierReadModel)
    {
        await readStoreDbContext.SubscriptionTiers.AddAsync(subscriptionTierReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }
    
    public async Task<List<SubscriptionTierReadModel>> GetAllSubscriptionTiers()
    {
        return await readStoreDbContext.SubscriptionTiers.ToListAsync();
    }
    
    public async Task<SubscriptionTierReadModel?> GetSubscriptionTierById(Guid id)
    {
        return await readStoreDbContext.SubscriptionTiers
            .Where(subscriptionTier => subscriptionTier.Id == new SubscriptionTierId(id))
            .SingleOrDefaultAsync();
    }
}