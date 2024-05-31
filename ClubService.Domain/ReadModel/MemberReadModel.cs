using ClubService.Domain.Event.Member;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.ReadModel;

public class MemberReadModel
{
    public MemberId MemberId { get; }
    public FullName Name { get; }
    public string Email { get; }
    public TennisClubId TennisClubId { get; }
    public MemberStatus Status { get; }
    
    private MemberReadModel(
        MemberId memberId,
        FullName name,
        string email,
        TennisClubId tennisClubId,
        MemberStatus status)
    {
        MemberId = memberId;
        Name = name;
        Email = email;
        TennisClubId = tennisClubId;
        Status = status;
    }
    
    public static MemberReadModel FromDomainEvent(MemberRegisteredEvent memberRegisteredEvent)
    {
        return new MemberReadModel(
            memberRegisteredEvent.MemberId,
            memberRegisteredEvent.Name,
            memberRegisteredEvent.Email,
            memberRegisteredEvent.TennisClubId,
            memberRegisteredEvent.Status
        );
    }
}