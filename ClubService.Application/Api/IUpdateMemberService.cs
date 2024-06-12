using ClubService.Application.Commands;

namespace ClubService.Application.Api;

public interface IUpdateMemberService
{
    Task<Guid> LockMember(Guid id);
    Task<Guid> UnlockMember(Guid id);
    Task<Guid> ChangeFullName(Guid id, string firstName, string lastName);
    Task<Guid> ChangeEmail(Guid id, string email);
    Task<Guid> UpdateMember(Guid id, MemberUpdateCommand memberUpdateCommand);
}