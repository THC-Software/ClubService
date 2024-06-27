namespace ClubService.Application.Api;

public interface IDeleteMemberService
{
    Task<Guid> DeleteMember(Guid id, string? jwtTennisClubId);
}