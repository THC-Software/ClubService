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
        switch (Status)
        {
            case MemberStatus.DELETED:
                throw new InvalidOperationException("Member is already deleted!");
            case MemberStatus.LOCKED:
                throw new InvalidOperationException("Member is already locked!");
            case MemberStatus.NONE:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        var memberLockedEvent = new MemberLockedEvent();
        
        var domainEnvelope = new DomainEnvelope<IMemberDomainEvent>(
            Guid.NewGuid(),
            MemberId.Id,
            EventType.MEMBER_LOCKED,
            EntityType.MEMBER,
            DateTime.UtcNow,
            memberLockedEvent
        );
        
        return [domainEnvelope];
    }
    
    public List<DomainEnvelope<IMemberDomainEvent>> ProcessMemberUnlockCommand()
    {
        switch (Status)
        {
            case MemberStatus.DELETED:
                throw new InvalidOperationException("Member is already deleted!");
            case MemberStatus.NONE:
                throw new InvalidOperationException("Member needs to be locked!");
            case MemberStatus.LOCKED:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        var memberUnlockedEvent = new MemberUnlockedEvent();
        
        var domainEnvelope = new DomainEnvelope<IMemberDomainEvent>(
            Guid.NewGuid(),
            MemberId.Id,
            EventType.MEMBER_UNLOCKED,
            EntityType.MEMBER,
            DateTime.UtcNow,
            memberUnlockedEvent
        );
        
        return [domainEnvelope];
    }
    
    public List<DomainEnvelope<IMemberDomainEvent>> ProcessMemberDeleteCommand()
    {
        if (Status.Equals(MemberStatus.DELETED))
        {
            throw new InvalidOperationException("Member is already deleted!");
        }
        
        var memberDeletedEvent = new MemberDeletedEvent();
        
        var domainEnvelope = new DomainEnvelope<IMemberDomainEvent>(
            Guid.NewGuid(),
            MemberId.Id,
            EventType.MEMBER_DELETED,
            EntityType.MEMBER,
            DateTime.UtcNow,
            memberDeletedEvent
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
            case EventType.MEMBER_DELETED:
                Apply((MemberDeletedEvent)domainEnvelope.EventData);
                break;
            case EventType.MEMBER_LOCKED:
                Apply((MemberLockedEvent)domainEnvelope.EventData);
                break;
            case EventType.MEMBER_UNLOCKED:
                Apply((MemberUnlockedEvent)domainEnvelope.EventData);
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
    
    // Parameter is only in method signature to distinguish the Apply method from the others
    private void Apply(MemberLockedEvent memberLockedEvent)
    {
        Status = MemberStatus.LOCKED;
    }
    
    // Parameter is only in method signature to distinguish the Apply method from the others
    private void Apply(MemberUnlockedEvent memberUnlockedEvent)
    {
        Status = MemberStatus.NONE;
    }
    
    // Parameter is only in method signature to distinguish the Apply method from the others
    private void Apply(MemberDeletedEvent memberDeletedEvent)
    {
        Status = MemberStatus.DELETED;
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