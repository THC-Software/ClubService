using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Event.Member;

// TODO: RENAME!
public class MemberAccountDomainEvent(
    MemberId id,
    FullName name,
    string email,
    bool isLocked,
    TennisClubId tennisClubId,
    bool isDeleted) : IMemberDomainEvent
{
    public MemberId Id { get; private set; } = id;
    public FullName Name { get; private set; } = name;
    public string Email { get; private set; } = email;
    public bool IsLocked { get; private set; } = isLocked;
    public TennisClubId TennisClubId { get; private set; } = tennisClubId;
    public bool IsDeleted { get; private set; } = isDeleted;
}