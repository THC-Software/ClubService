using ClubService.Application.Commands;

namespace ClubService.Application.Api;

public interface ICreateMemberService
{
    Task<string> CreateMember(MemberCreateCommand memberCreateCommand);
}