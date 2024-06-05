using ClubService.Application.Commands;

namespace ClubService.Application.Api;

public interface IRegisterMemberService
{
    Task<Guid> RegisterMember(MemberRegisterCommand memberRegisterCommand);
}