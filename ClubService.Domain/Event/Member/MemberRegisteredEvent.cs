using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Event.Member;

public class MemberRegisteredEvent(
    MemberId memberId,
    FullName name,
    string email,
    TennisClubId tennisClubId,
    MemberStatus memberStatus) : IMemberDomainEvent
{
    public MemberId MemberId { get; private set; } = memberId;
    public FullName Name { get; private set; } = name;
    public string Email { get; private set; } = email;
    public TennisClubId TennisClubId { get; private set; } = tennisClubId;
    public MemberStatus MemberStatus { get; private set; } = memberStatus;
}