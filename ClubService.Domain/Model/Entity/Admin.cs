using ClubService.Domain.Event;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Model.Entity;

public class Admin
{
    public AdminId Id { get; }
    public string Username { get; }
    public FullName Name { get; }
    public TennisClubId TennisClubId { get; }
    public bool IsDeleted { get; }

    private Admin(AdminId id, string username, FullName name, TennisClubId tennisClubId, bool isDeleted)
    {
        Id = id;
        Username = username;
        Name = name;
        TennisClubId = tennisClubId;
        IsDeleted = isDeleted;
    }

    public static Admin Create(AdminId id, string username, FullName name, TennisClubId tennisClubId)
    {
        return new Admin(id, username, name, tennisClubId, isDeleted: false);
    }

    public void Apply(DomainEnvelope<IAdminDomainEvent> domainEnvelope)
    {
        switch (domainEnvelope.EventType)
        {
            case EventType.ADMIN_ACCOUNT_REGISTERED:
                break;
        }
    }
    
    
    protected bool Equals(Admin other)
    {
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Admin)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}