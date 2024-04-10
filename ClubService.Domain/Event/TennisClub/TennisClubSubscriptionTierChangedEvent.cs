using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Event.TennisClub;

public class TennisClubSubscriptionTierChangedEvent(
    Guid eventId,
    Guid entityId,
    EventType eventType,
    EntityType entityType,
    SubscriptionTierId subscriptionTierId) : Event(eventId, entityId, eventType, entityType)
{
    public SubscriptionTierId SubscriptionTierId { get; } = subscriptionTierId;
}