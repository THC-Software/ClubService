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
    public AdminStatus AdminStatus { get; private set; }
    
    public List<DomainEnvelope<IAdminDomainEvent>> ProcessAdminRegisteredCommand(
        string username,
        FullName name,
        TennisClubId tennisClubId)
    {
        var adminRegisteredEvent = new AdminRegisteredEvent(
            new AdminId(Guid.NewGuid()),
            username,
            name,
            tennisClubId,
            AdminStatus.NONE
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
    
    public void Apply(DomainEnvelope<IAdminDomainEvent> domainEnvelope)
    {
        switch (domainEnvelope.EventType)
        {
            case EventType.ADMIN_REGISTERED:
                Apply((AdminRegisteredEvent)domainEnvelope.EventData);
                break;
        }
    }
    
    private void Apply(AdminRegisteredEvent adminRegisteredEvent)
    {
        AdminId = adminRegisteredEvent.AdminId;
        Username = adminRegisteredEvent.Username;
        Name = adminRegisteredEvent.Name;
        TennisClubId = adminRegisteredEvent.TennisClubId;
        AdminStatus = adminRegisteredEvent.AdminStatus;
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