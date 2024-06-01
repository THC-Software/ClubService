using ClubService.Domain.ReadModel;

namespace ClubService.Domain.Repository;

public interface ISubscriptionTierReadModelRepository
{
    Task Add(SubscriptionTierReadModel subscriptionTierReadModel);
    Task<List<SubscriptionTierReadModel>> GetAllSubscriptionTiers();
}