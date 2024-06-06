using ClubService.Application.Commands;

namespace ClubService.Application.Api;

public interface IUpdateTennisClubService
{
    Task<Guid> LockTennisClub(Guid id);
    Task<Guid> UnlockTennisClub(Guid id);
    Task<Guid> UpdateTennisClub(Guid id, TennisClubUpdateCommand tennisClubUpdateCommand);
}