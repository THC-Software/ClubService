using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.ReadModel;

public class SubscriptionTierReadModel
{
    public SubscriptionTierId SubscriptionTierId { get; }
    public string Name { get; }
    public int MaxMemberCount { get; }
    
    private SubscriptionTierReadModel(SubscriptionTierId subscriptionTierId, string name, int maxMemberCount)
    {
        SubscriptionTierId = subscriptionTierId;
        Name = name;
        MaxMemberCount = maxMemberCount;
    }
    
    public static SubscriptionTierReadModel FromDomainEvent(SubscriptionTierCreatedEvent subscriptionTierCreatedEvent)
    {
        return new SubscriptionTierReadModel(
            subscriptionTierCreatedEvent.Id,
            subscriptionTierCreatedEvent.Name,
            subscriptionTierCreatedEvent.MaxMemberCount
        );
    }
}