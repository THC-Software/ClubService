using ClubService.Application.Commands;

namespace ClubService.Application.Api;

public interface IUpdateMemberService
{
    Task<Guid> LockMember(Guid id, string? jwtTennisClubId);
    Task<Guid> UnlockMember(Guid id, string? jwtTennisClubId);

    Task<Guid> UpdateMember(
        Guid id,
        MemberUpdateCommand memberUpdateCommand,
        string? jwtUserId,
        string? jwtTennisClubId);
}