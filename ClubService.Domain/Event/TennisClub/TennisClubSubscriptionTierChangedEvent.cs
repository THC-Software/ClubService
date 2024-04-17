using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Event.TennisClub;

public class TennisClubSubscriptionTierChangedEvent(SubscriptionTierId subscriptionTierId) : ITennisClubDomainEvent
{
    public SubscriptionTierId SubscriptionTierId { get; } = subscriptionTierId;
}