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
        SubscriptionTierId subscriptionTierIdStr)
    {
        var tennisClubRegisteredEvent = new TennisClubRegisteredEvent(
            new TennisClubId(Guid.NewGuid()),
            name,
            subscriptionTierIdStr,
            TennisClubStatus.ACTIVE
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
        if (Status.Equals(TennisClubStatus.ACTIVE))
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

    public List<DomainEnvelope<ITennisClubDomainEvent>> ProcessTennisClubUpdateCommand(
        string? name,
        SubscriptionTierId? subscriptionTierId)
    {
        // TODO: Check state
        var domainEnvelopes = new List<DomainEnvelope<ITennisClubDomainEvent>>();

        if (!string.IsNullOrWhiteSpace(name) && !name.Equals(Name))
        {
            var nameChangedEvent = new TennisClubNameChangedEvent(name);

            var domainEnvelope = new DomainEnvelope<ITennisClubDomainEvent>(
                Guid.NewGuid(),
                TennisClubId.Id,
                EventType.TENNIS_CLUB_NAME_CHANGED,
                EntityType.TENNIS_CLUB,
                DateTime.UtcNow,
                nameChangedEvent
            );

            domainEnvelopes.Add(domainEnvelope);
        }

        if (subscriptionTierId != null && !subscriptionTierId.Equals(SubscriptionTierId))
        {
            var subscriptionTierChangedEvent = new TennisClubSubscriptionTierChangedEvent(subscriptionTierId);

            var domainEnvelope = new DomainEnvelope<ITennisClubDomainEvent>(
                Guid.NewGuid(),
                TennisClubId.Id,
                EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED,
                EntityType.TENNIS_CLUB,
                DateTime.UtcNow,
                subscriptionTierChangedEvent
            );

            domainEnvelopes.Add(domainEnvelope);
        }

        return domainEnvelopes;
    }

    public List<DomainEnvelope<ITennisClubDomainEvent>> ProcessTennisClubDeleteCommand()
    {
        if (Status.Equals(TennisClubStatus.DELETED))
        {
            throw new InvalidOperationException("Tennis Club is already deleted!");
        }

        var tennisClubDeletedEvent = new TennisClubDeletedEvent();

        var domainEnvelope = new DomainEnvelope<ITennisClubDomainEvent>(
            Guid.NewGuid(),
            TennisClubId.Id,
            EventType.TENNIS_CLUB_DELETED,
            EntityType.TENNIS_CLUB,
            DateTime.UtcNow,
            tennisClubDeletedEvent
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
            case EventType.TENNIS_CLUB_DELETED:
                Apply((TennisClubDeletedEvent)domainEnvelope.EventData);
                break;
            case EventType.ADMIN_REGISTERED:
            case EventType.ADMIN_DELETED:
            case EventType.SUBSCRIPTION_TIER_CREATED:
            case EventType.MEMBER_REGISTERED:
            case EventType.MEMBER_DELETED:
            case EventType.MEMBER_LOCKED:
            case EventType.MEMBER_UNLOCKED:
            case EventType.MEMBER_FULL_NAME_CHANGED:
            case EventType.MEMBER_EMAIL_CHANGED:
            case EventType.ADMIN_FULL_NAME_CHANGED:
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
        Status = TennisClubStatus.ACTIVE;
    }

    private void Apply(TennisClubSubscriptionTierChangedEvent tennisClubSubscriptionTierChangedEvent)
    {
        SubscriptionTierId = tennisClubSubscriptionTierChangedEvent.SubscriptionTierId;
    }

    private void Apply(TennisClubNameChangedEvent tennisClubNameChangedEvent)
    {
        Name = tennisClubNameChangedEvent.Name;
    }

    // Parameter is only in method signature to distinguish the Apply method from the others
    private void Apply(TennisClubDeletedEvent tennisClubDeletedEvent)
    {
        Status = TennisClubStatus.DELETED;
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