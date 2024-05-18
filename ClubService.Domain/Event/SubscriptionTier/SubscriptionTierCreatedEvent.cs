using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Event.SubscriptionTier;

public class SubscriptionTierCreatedEvent(SubscriptionTierId id, string name, int maxMemberCount)
    : ISubscriptionTierDomainEvent
{
    public SubscriptionTierId Id { get; } = id;
    public string Name { get; } = name;
    public int MaxMemberCount { get; } = maxMemberCount;
}