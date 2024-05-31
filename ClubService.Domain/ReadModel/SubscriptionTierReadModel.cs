using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.ReadModel;

public class SubscriptionTierReadModel
{
    public SubscriptionTierId Id { get; }
    public string Name { get; }
    public int MaxMemberCount { get; }
    
    private SubscriptionTierReadModel(SubscriptionTierId id, string name, int maxMemberCount)
    {
        Id = id;
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