using ClubService.Domain.Event;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Model.Entity;

public class SubscriptionTier
{
    private SubscriptionTierId Id { get; set; } = null!;
    public string Name { get; private set; } = null!;
    public int MaxMemberCount { get; private set; }
    
    public void Apply(DomainEnvelope<ISubscriptionTierDomainEvent> domainEnvelope)
    {
        switch (domainEnvelope.EventType)
        {
            case EventType.SUBSCRIPTION_TIER_CREATED:
                Apply((SubscriptionTierCreatedEvent)domainEnvelope.EventData);
                break;
            case EventType.TENNIS_CLUB_REGISTERED:
            case EventType.MEMBER_REGISTERED:
            case EventType.MEMBER_DELETED:
            case EventType.ADMIN_REGISTERED:
            case EventType.ADMIN_DELETED:
            case EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED:
            case EventType.MEMBER_LOCKED:
            case EventType.MEMBER_UNLOCKED:
            case EventType.MEMBER_UPDATED:
            case EventType.TENNIS_CLUB_LOCKED:
            case EventType.TENNIS_CLUB_UNLOCKED:
            default:
                throw new ArgumentException(
                    $"{nameof(domainEnvelope.EventType)} is not supported for the entity TennisClub!");
        }
    }
    
    private void Apply(SubscriptionTierCreatedEvent subscriptionTierCreatedEvent)
    {
        Id = subscriptionTierCreatedEvent.Id;
        Name = subscriptionTierCreatedEvent.Name;
        MaxMemberCount = subscriptionTierCreatedEvent.MaxMemberCount;
    }
    
    protected bool Equals(SubscriptionTier other)
    {
        return Id.Equals(other.Id);
    }
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        
        if (obj.GetType() != GetType())
        {
            return false;
        }
        
        return Equals((SubscriptionTier)obj);
    }
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}