using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Model.Entity;

public class TennisClub
{
    public TennisClubId TennisClubId { get; private set; }
    public string Name { get; private set; }
    public bool IsLocked { get; private set; }
    public SubscriptionTierId SubscriptionTierId { get; private set; }
    public List<MemberId> MemberIds { get; private set; }

    private TennisClub(TennisClubId tennisClubId)
    {
        TennisClubId = tennisClubId;
    }

    public static TennisClub Create(TennisClubId id)
    {
        return new TennisClub(id);
    }

    public List<DomainEnvelope<ITennisClubDomainEvent>> ProcessTennisClubRegisterCommand(string name,
        string subscriptionTierIdStr)
    {
        var subscriptionTierId = new SubscriptionTierId(new Guid(subscriptionTierIdStr));
        var memberIds = new List<MemberId>();

        var tennisClubRegisteredEvent = new TennisClubRegisteredEvent(
            TennisClubId,
            name,
            false,
            subscriptionTierId,
            memberIds
        );

        var domainEnvelope = new DomainEnvelope<ITennisClubDomainEvent>(
            Guid.NewGuid(),
            TennisClubId.Id,
            EventType.TENNIS_CLUB_REGISTERED,
            EntityType.TENNIS_CLUB,
            DateTime.Now,
            tennisClubRegisteredEvent
        );

        return [domainEnvelope];
    }

    public void Apply(DomainEnvelope<ITennisClubDomainEvent> domainEnvelope)
    {
        switch (domainEnvelope.EventType)
        {
            case EventType.TENNIS_CLUB_REGISTERED:
                Apply((TennisClubRegisteredEvent)domainEnvelope.DomainEvent);
                break;
            case EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED:
                break;
            case EventType.TENNIS_CLUB_LOCKED:
                break;
            case EventType.TENNIS_CLUB_UNLOCKED:
                break;
            case EventType.MEMBER_ACCOUNT_CREATED:
            case EventType.MEMBER_ACCOUNT_LIMIT_EXCEEDED:
            case EventType.MEMBER_ACCOUNT_DELETED:
            case EventType.ADMIN_ACCOUNT_CREATED:
            case EventType.ADMIN_ACCOUNT_DELETED:
            case EventType.MEMBER_ACCOUNT_LOCKED:
            case EventType.MEMBER_ACCOUNT_UNLOCKED:
            case EventType.MEMBER_ACCOUNT_UPDATED:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Apply(TennisClubRegisteredEvent tennisClubRegisteredEvent)
    {
        TennisClubId = tennisClubRegisteredEvent.TennisClubId;
        Name = tennisClubRegisteredEvent.Name;
        IsLocked = tennisClubRegisteredEvent.IsLocked;
        SubscriptionTierId = tennisClubRegisteredEvent.SubscriptionTierId;
        MemberIds = tennisClubRegisteredEvent.MemberIds;
    }

    protected bool Equals(TennisClub other)
    {
        return TennisClubId.Equals(other.TennisClubId);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TennisClub)obj);
    }

    public override int GetHashCode()
    {
        return TennisClubId.GetHashCode();
    }
}