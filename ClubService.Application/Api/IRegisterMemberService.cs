using ClubService.Application.Commands;

namespace ClubService.Application.Api;

public interface IRegisterMemberService
{
    Task<string> RegisterMember(MemberRegisterCommand memberRegisterCommand);
}