using ClubService.Application.Commands;

namespace ClubService.Application.Api;

public interface IRegisterTennisClubService
{
    Task<Guid> RegisterTennisClub(TennisClubRegisterCommand tennisClubRegisterCommand);
}