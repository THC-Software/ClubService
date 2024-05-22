using ClubService.Domain.Event;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Model.Entity;

public class Member
{
    public MemberId MemberId { get; private set; } = null!;
    public FullName Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public TennisClubId TennisClubId { get; private set; } = null!;
    public MemberStatus Status { get; private set; }
    
    public List<DomainEnvelope<IMemberDomainEvent>> ProcessMemberRegisterCommand(
        string firstName,
        string lastName,
        string email,
        string tennisClubId)
    {
        var memberRegisteredEvent = new MemberRegisteredEvent(
            new MemberId(Guid.NewGuid()),
            new FullName(firstName, lastName),
            email,
            new TennisClubId(new Guid(tennisClubId)),
            MemberStatus.NONE
        );
        
        var domainEnvelop = new DomainEnvelope<IMemberDomainEvent>(
            Guid.NewGuid(),
            memberRegisteredEvent.MemberId.Id,
            EventType.MEMBER_REGISTERED,
            EntityType.MEMBER,
            DateTime.UtcNow,
            memberRegisteredEvent
        );
        
        return [domainEnvelop];
    }
    
    public List<DomainEnvelope<IMemberDomainEvent>> ProcessMemberLockCommand()
    {
        if (Status.Equals(MemberStatus.LOCKED))
        {
            throw new InvalidOperationException("Member is already locked!");
        }
        
        var memberLockedEvent = new MemberLockedEvent();
        
        var domainEnvelope = new DomainEnvelope<IMemberDomainEvent>(
            Guid.NewGuid(),
            MemberId.Id,
            EventType.TENNIS_CLUB_LOCKED,
            EntityType.TENNIS_CLUB,
            DateTime.UtcNow,
            memberLockedEvent
        );
        
        return [domainEnvelope];
    }
    
    public void Apply(DomainEnvelope<IMemberDomainEvent> domainEnvelope)
    {
        switch (domainEnvelope.EventType)
        {
            case EventType.MEMBER_REGISTERED:
                Apply((MemberRegisteredEvent)domainEnvelope.EventData);
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
            case EventType.ADMIN_REGISTERED:
            case EventType.ADMIN_DELETED:
            case EventType.TENNIS_CLUB_REGISTERED:
            case EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED:
            case EventType.TENNIS_CLUB_LOCKED:
            case EventType.TENNIS_CLUB_UNLOCKED:
            default:
                throw new ArgumentException(
                    $"{nameof(domainEnvelope.EventType)} is not supported for the entity Member!");
        }
    }
    
    private void Apply(MemberRegisteredEvent memberRegisteredEvent)
    {
        MemberId = memberRegisteredEvent.MemberId;
        Name = memberRegisteredEvent.Name;
        Email = memberRegisteredEvent.Email;
        TennisClubId = memberRegisteredEvent.TennisClubId;
        Status = memberRegisteredEvent.Status;
    }
    
    protected bool Equals(Member other)
    {
        return MemberId.Equals(other.MemberId);
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
        
        return Equals((Member)obj);
    }
    
    public override int GetHashCode()
    {
        return MemberId.GetHashCode();
    }
}