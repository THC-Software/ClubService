using ClubService.Domain.Event;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Model.Entity;

public class Member
{
    public MemberId MemberId { get; private set; } = null!;
    public FullName Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public bool IsLocked { get; private set; }
    public TennisClubId TennisClubId { get; private set; } = null!;
    public bool IsDeleted { get; private set; }
    
    public List<DomainEnvelope<IMemberDomainEvent>> ProcessMemberCreatedCommand(
        string firstName,
        string lastName,
        string email,
        string tennisClubId)
    {
        var memberCreatedEvent = new MemberCreatedEvent(
            new MemberId(Guid.NewGuid()),
            new FullName(firstName, lastName),
            email,
            false,
            new TennisClubId(new Guid(tennisClubId)),
            false
        );
        
        var domainEnvelop = new DomainEnvelope<IMemberDomainEvent>(
            Guid.NewGuid(),
            memberCreatedEvent.MemberId.Id,
            EventType.MEMBER_CREATED,
            EntityType.MEMBER,
            DateTime.UtcNow,
            memberCreatedEvent
        );
        
        return [domainEnvelop];
    }
    
    public void Apply(DomainEnvelope<IMemberDomainEvent> domainEnvelope)
    {
        switch (domainEnvelope.EventType)
        {
            case EventType.MEMBER_CREATED:
                Apply((MemberCreatedEvent)domainEnvelope.EventData);
                break;
            case EventType.MEMBER_LIMIT_EXCEEDED:
                break;
            case EventType.MEMBER_DELETED:
                break;
            case EventType.MEMBER_LOCKED:
                break;
            case EventType.MEMBER_UNLOCKED:
                break;
            case EventType.MEMBER_UPDATED:
                break;
            case EventType.ADMIN_ACCOUNT_CREATED:
            case EventType.ADMIN_ACCOUNT_DELETED:
            case EventType.TENNIS_CLUB_REGISTERED:
            case EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED:
            case EventType.TENNIS_CLUB_LOCKED:
            case EventType.TENNIS_CLUB_UNLOCKED:
            default:
                throw new ArgumentException(
                    $"{nameof(domainEnvelope.EventType)} is not supported for the entity Member!");
        }
    }
    
    private void Apply(MemberCreatedEvent memberCreatedEvent)
    {
        MemberId = memberCreatedEvent.MemberId;
        Name = memberCreatedEvent.Name;
        Email = memberCreatedEvent.Email;
        IsLocked = memberCreatedEvent.IsLocked;
        TennisClubId = memberCreatedEvent.TennisClubId;
        IsDeleted = memberCreatedEvent.IsDeleted;
    }
    
    protected bool Equals(Member other)
    {
        return MemberId.Equals(other.MemberId);
    }
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Member)obj);
    }
    
    public override int GetHashCode()
    {
        return MemberId.GetHashCode();
    }
}