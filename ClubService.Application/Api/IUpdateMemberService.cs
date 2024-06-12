using ClubService.Application.Commands;

namespace ClubService.Application.Api;

public interface IUpdateMemberService
{
    Task<Guid> LockMember(Guid id);
    Task<Guid> UnlockMember(Guid id);
    Task<Guid> UpdateMember(Guid id, MemberUpdateCommand memberUpdateCommand);
}