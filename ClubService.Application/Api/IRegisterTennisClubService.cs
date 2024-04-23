using ClubService.Application.Commands;

namespace ClubService.Application.Api;

public interface IRegisterTennisClubService
{
    Task<string> CreateTennisClub(TennisClubRegisterCommand tennisClubRegisterCommand);
}