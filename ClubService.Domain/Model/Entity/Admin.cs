using ClubService.Domain.Event;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Model.Entity;

public class Admin
{
    public AdminId AdminId { get; private set; } = null!;
    public string Username { get; private set; } = null!;
    public FullName Name { get; private set; } = null!;
    public TennisClubId TennisClubId { get; private set; } = null!;
    public AdminStatus Status { get; private set; }
    
    public List<DomainEnvelope<IAdminDomainEvent>> ProcessAdminRegisterCommand(
        string username,
        FullName name,
        TennisClubId tennisClubId)
    {
        var adminRegisteredEvent = new AdminRegisteredEvent(
            new AdminId(Guid.NewGuid()),
            username,
            name,
            tennisClubId,
            AdminStatus.ACTIVE
        );
        
        var domainEnvelope = new DomainEnvelope<IAdminDomainEvent>(
            Guid.NewGuid(),
            adminRegisteredEvent.AdminId.Id,
            EventType.ADMIN_REGISTERED,
            EntityType.ADMIN,
            DateTime.UtcNow,
            adminRegisteredEvent
        );
        
        return [domainEnvelope];
    }
    
    public List<DomainEnvelope<IAdminDomainEvent>> ProcessAdminDeleteCommand()
    {
        if (Status.Equals(AdminStatus.DELETED))
        {
            throw new InvalidOperationException("Admin is already deleted!");
        }
        
        var adminDeletedEvent = new AdminDeletedEvent();
        
        var domainEnvelope = new DomainEnvelope<IAdminDomainEvent>(
            Guid.NewGuid(),
            AdminId.Id,
            EventType.ADMIN_DELETED,
            EntityType.ADMIN,
            DateTime.UtcNow,
            adminDeletedEvent
        );
        
        return [domainEnvelope];
    }
    
    public List<DomainEnvelope<IAdminDomainEvent>> ProcessAdminChangeFullNameCommand(FullName fullName)
    {
        if (Status.Equals(AdminStatus.DELETED))
        {
            throw new InvalidOperationException("Admin is already deleted!");
        }
        if (fullName.Equals(Name))
        {
            throw new InvalidOperationException("This name is already set!");
        }
        
        var adminChangedFullNameEvent = new AdminFullNameChangedEvent(fullName);
        
        var domainEnvelope = new DomainEnvelope<IAdminDomainEvent>(
            Guid.NewGuid(),
            AdminId.Id,
            EventType.ADMIN_FULL_NAME_CHANGED,
            EntityType.ADMIN,
            DateTime.UtcNow,
            adminChangedFullNameEvent
        );
        
        return [domainEnvelope];
    }
    
    public void Apply(DomainEnvelope<IAdminDomainEvent> domainEnvelope)
    {
        switch (domainEnvelope.EventType)
        {
            case EventType.ADMIN_REGISTERED:
                Apply((AdminRegisteredEvent)domainEnvelope.EventData);
                break;
            case EventType.ADMIN_DELETED:
                Apply((AdminDeletedEvent)domainEnvelope.EventData);
                break;
            case EventType.ADMIN_FULL_NAME_CHANGED:
                Apply((AdminFullNameChangedEvent)domainEnvelope.EventData);
                break;
            case EventType.TENNIS_CLUB_REGISTERED:
            case EventType.MEMBER_REGISTERED:
            case EventType.MEMBER_DELETED:
            case EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED:
            case EventType.MEMBER_LOCKED:
            case EventType.MEMBER_UNLOCKED:
            case EventType.MEMBER_FULL_NAME_CHANGED:
            case EventType.TENNIS_CLUB_LOCKED:
            case EventType.TENNIS_CLUB_UNLOCKED:
            case EventType.SUBSCRIPTION_TIER_CREATED:
            case EventType.TENNIS_CLUB_NAME_CHANGED:
            case EventType.TENNIS_CLUB_DELETED:
            case EventType.MEMBER_EMAIL_CHANGED:
            default:
                throw new ArgumentException(
                    $"{nameof(domainEnvelope.EventType)} is not supported for the entity Admin!");
        }
    }
    
    private void Apply(AdminRegisteredEvent adminRegisteredEvent)
    {
        AdminId = adminRegisteredEvent.AdminId;
        Username = adminRegisteredEvent.Username;
        Name = adminRegisteredEvent.Name;
        TennisClubId = adminRegisteredEvent.TennisClubId;
        Status = adminRegisteredEvent.Status;
    }
    
    private void Apply(AdminFullNameChangedEvent adminFullNameChangedEvent)
    {
        Name = adminFullNameChangedEvent.Name;
    }
    
    // Parameter is only in method signature to distinguish the Apply method from the others
    private void Apply(AdminDeletedEvent adminDeletedEvent)
    {
        Status = AdminStatus.DELETED;
    }
    
    protected bool Equals(Admin other)
    {
        return AdminId.Equals(other.AdminId);
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
        
        return Equals((Admin)obj);
    }
    
    public override int GetHashCode()
    {
        return AdminId.GetHashCode();
    }
}