using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Model.Entity;

public class Admin
{
    public AdminId Id { get; private set; }
    public string Username { get; private set; }
    public FullName Name { get; private set; }
    public TennisClubId TennisClubId { get; private set; }

    private Admin() { }

    private Admin(AdminId id, string username, FullName name, TennisClubId tennisClubId)
    {
        Id = id;
        Username = username;
        Name = name;
        TennisClubId = tennisClubId;
    }

    public static Admin Create(AdminId id, string username, FullName name, TennisClubId tennisClubId)
    {
        return new Admin(id, username, name, tennisClubId);
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