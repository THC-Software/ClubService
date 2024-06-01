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
}