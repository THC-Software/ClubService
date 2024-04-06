namespace ClubService.Domain.Model;

public class Member
{
    public MemberId Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public bool IsLocked { get; private set; }
    public TennisClubId TennisClubId { get; private set; }
    
    private Member() { }

    private Member(MemberId id, string name, string email, bool isLocked, TennisClubId tennisClubId)
    {
        Id = id;
        Name = name;
        Email = email;
        IsLocked = isLocked;
        TennisClubId = tennisClubId;
    }

    public static Member Create(MemberId id, string name, string email, bool isLocked, TennisClubId tennisClubId)
    {
        return new Member(id, name, email, isLocked, tennisClubId);
    }
}