namespace ClubService.Application.Api;

public interface IDeleteTennisClubService
{
    Task<Guid> DeleteTennisClub(Guid id, string? jwtRole, string? jwtTennisClubId);
}