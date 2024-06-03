using ClubService.Domain.Event.Member;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.ReadModel;

public class MemberReadModel
{
    public MemberId MemberId { get; } = null!;
    public FullName Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public TennisClubId TennisClubId { get; } = null!;
    public MemberStatus Status { get; private set; }
    
    // needed by efcore
    private MemberReadModel()
    {
    }
    
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
    
    public void Lock()
    {
        Status = MemberStatus.LOCKED;
    }
    
    public void Unlock()
    {
        Status = MemberStatus.ACTIVE;
    }
    
    public void ChangeFullName(FullName name)
    {
        Name = name;
    }
    
    public void ChangeEmail(string email)
    {
        Email = email;
    }
}