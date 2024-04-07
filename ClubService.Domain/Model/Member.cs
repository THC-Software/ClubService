namespace ClubService.Domain.Model;

public class Member
{
    public MemberId Id { get; private set; }
    public FullName Name { get; private set; }
    public string Email { get; private set; }
    public bool IsLocked { get; private set; }
    public TennisClubId TennisClubId { get; private set; }
    
    private Member() { }

    private Member(MemberId id, FullName name, string email, bool isLocked, TennisClubId tennisClubId)
    {
        Id = id;
        Name = name;
        Email = email;
        IsLocked = isLocked;
        TennisClubId = tennisClubId;
    }

    public static Member Create(MemberId id, FullName name, string email, bool isLocked, TennisClubId tennisClubId)
    {
        return new Member(id, name, email, isLocked, tennisClubId);
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