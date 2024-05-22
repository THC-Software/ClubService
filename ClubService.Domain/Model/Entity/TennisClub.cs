using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Model.Entity;

public class TennisClub
{
    public TennisClubId TennisClubId { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public SubscriptionTierId SubscriptionTierId { get; private set; } = null!;
    public TennisClubStatus Status { get; private set; }
    
    public List<DomainEnvelope<ITennisClubDomainEvent>> ProcessTennisClubRegisterCommand(
        string name,
        string subscriptionTierIdStr)
    {
        var tennisClubRegisteredEvent = new TennisClubRegisteredEvent(
            new TennisClubId(Guid.NewGuid()),
            name,
            new SubscriptionTierId(new Guid(subscriptionTierIdStr)),
            TennisClubStatus.NONE
        );
        
        var domainEnvelope = new DomainEnvelope<ITennisClubDomainEvent>(
            Guid.NewGuid(),
            tennisClubRegisteredEvent.TennisClubId.Id,
            EventType.TENNIS_CLUB_REGISTERED,
            EntityType.TENNIS_CLUB,
            DateTime.UtcNow,
            tennisClubRegisteredEvent
        );
        
        return [domainEnvelope];
    }
    
    public List<DomainEnvelope<ITennisClubDomainEvent>> ProcessTennisClubLockCommand()
    {
        if (Status.Equals(TennisClubStatus.LOCKED))
        {
            throw new InvalidOperationException("Tennis Club is already locked!");
        }
        
        var tennisClubLockedEvent = new TennisClubLockedEvent();
        
        var domainEnvelope = new DomainEnvelope<ITennisClubDomainEvent>(
            Guid.NewGuid(),
            TennisClubId.Id,
            EventType.TENNIS_CLUB_LOCKED,
            EntityType.TENNIS_CLUB,
            DateTime.UtcNow,
            tennisClubLockedEvent
        );
        
        return [domainEnvelope];
    }
    
    public List<DomainEnvelope<ITennisClubDomainEvent>> ProcessTennisClubUnlockCommand()
    {
        if (Status.Equals(TennisClubStatus.NONE))
        {
            throw new InvalidOperationException("Tennis Club needs to be locked!");
        }
        
        var tennisClubUnlockedEvent = new TennisClubUnlockedEvent();
        
        var domainEnvelope = new DomainEnvelope<ITennisClubDomainEvent>(
            Guid.NewGuid(),
            TennisClubId.Id,
            EventType.TENNIS_CLUB_UNLOCKED,
            EntityType.TENNIS_CLUB,
            DateTime.UtcNow,
            tennisClubUnlockedEvent
        );
        
        return [domainEnvelope];
    }
    
    public List<DomainEnvelope<ITennisClubDomainEvent>> ProcessTennisClubChangeSubscriptionTierCommand(
        string subscriptionTierIdStr)
    {
        if (subscriptionTierIdStr.Equals(SubscriptionTierId.Id.ToString()))
        {
            throw new InvalidOperationException("This subscription tier is already set!");
        }
        
        var subscriptionTierId = new SubscriptionTierId(new Guid(subscriptionTierIdStr));
        
        var subscriptionTierChangedEvent = new TennisClubSubscriptionTierChangedEvent(subscriptionTierId);
        
        var domainEnvelope = new DomainEnvelope<ITennisClubDomainEvent>(
            Guid.NewGuid(),
            TennisClubId.Id,
            EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED,
            EntityType.TENNIS_CLUB,
            DateTime.UtcNow,
            subscriptionTierChangedEvent
        );
        
        return [domainEnvelope];
    }
    
    public List<DomainEnvelope<ITennisClubDomainEvent>> ProcessTennisClubChangeNameCommand(string name)
    {
        if (name.Equals(Name))
        {
            throw new InvalidOperationException("This name is already set!");
        }
        
        var nameChangedEvent = new TennisClubNameChangedEvent(name);
        
        var domainEnvelope = new DomainEnvelope<ITennisClubDomainEvent>(
            Guid.NewGuid(),
            TennisClubId.Id,
            EventType.TENNIS_CLUB_NAME_CHANGED,
            EntityType.TENNIS_CLUB,
            DateTime.UtcNow,
            nameChangedEvent
        );
        
        return [domainEnvelope];
    }
    
    public void Apply(DomainEnvelope<ITennisClubDomainEvent> domainEnvelope)
    {
        switch (domainEnvelope.EventType)
        {
            case EventType.TENNIS_CLUB_REGISTERED:
                Apply((TennisClubRegisteredEvent)domainEnvelope.EventData);
                break;
            case EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED:
                Apply((TennisClubSubscriptionTierChangedEvent)domainEnvelope.EventData);
                break;
            case EventType.TENNIS_CLUB_LOCKED:
                Apply((TennisClubLockedEvent)domainEnvelope.EventData);
                break;
            case EventType.TENNIS_CLUB_UNLOCKED:
                Apply((TennisClubUnlockedEvent)domainEnvelope.EventData);
                break;
            case EventType.TENNIS_CLUB_NAME_CHANGED:
                Apply((TennisClubNameChangedEvent)domainEnvelope.EventData);
                break;
            case EventType.ADMIN_REGISTERED:
            case EventType.ADMIN_DELETED:
            case EventType.SUBSCRIPTION_TIER_CREATED:
            case EventType.MEMBER_REGISTERED:
            case EventType.MEMBER_LIMIT_EXCEEDED:
            case EventType.MEMBER_DELETED:
            case EventType.MEMBER_LOCKED:
            case EventType.MEMBER_UNLOCKED:
            case EventType.MEMBER_UPDATED:
            default:
                throw new ArgumentException(
                    $"{nameof(domainEnvelope.EventType)} is not supported for the entity TennisClub!");
        }
    }
    
    private void Apply(TennisClubRegisteredEvent tennisClubRegisteredEvent)
    {
        TennisClubId = tennisClubRegisteredEvent.TennisClubId;
        Name = tennisClubRegisteredEvent.Name;
        SubscriptionTierId = tennisClubRegisteredEvent.SubscriptionTierId;
        Status = tennisClubRegisteredEvent.Status;
    }
    
    // Parameter is only in method signature to distinguish the Apply method from the others
    private void Apply(TennisClubLockedEvent tennisClubLockedEvent)
    {
        Status = TennisClubStatus.LOCKED;
    }
    
    // Parameter is only in method signature to distinguish the Apply method from the others
    private void Apply(TennisClubUnlockedEvent tennisClubUnlockedEvent)
    {
        Status = TennisClubStatus.NONE;
    }
    
    private void Apply(TennisClubSubscriptionTierChangedEvent tennisClubSubscriptionTierChangedEvent)
    {
        SubscriptionTierId = tennisClubSubscriptionTierChangedEvent.SubscriptionTierId;
    }
    
    private void Apply(TennisClubNameChangedEvent tennisClubNameChangedEvent)
    {
        Name = tennisClubNameChangedEvent.Name;
    }
    
    protected bool Equals(TennisClub other)
    {
        return TennisClubId.Equals(other.TennisClubId);
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
        
        return Equals((TennisClub)obj);
    }
    
    public override int GetHashCode()
    {
        return TennisClubId.GetHashCode();
    }
}