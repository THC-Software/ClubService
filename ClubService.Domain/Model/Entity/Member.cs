using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Model.Entity;

public class Member
{
    public MemberId Id { get; }
    public FullName Name { get; }
    public string Email { get; }
    public bool IsLocked { get; }
    public TennisClubId TennisClubId { get; }
    public bool IsDeleted { get; }
    
    private Member(MemberId id, FullName name, string email, bool isLocked, TennisClubId tennisClubId, bool isDeleted)
    {
        Id = id;
        Name = name;
        Email = email;
        IsLocked = isLocked;
        TennisClubId = tennisClubId;
        IsDeleted = isDeleted;
    }

    public static Member Create(MemberId id, FullName name, string email, TennisClubId tennisClubId)
    {
        return new Member(id, name, email, isLocked: false, tennisClubId, isDeleted: false);
    }

    protected bool Equals(Member other)
    {
        return Id.Equals(other.Id);
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
        return Id.GetHashCode();
    }
}