namespace ClubService.Application.Api;

public interface IUpdateTennisClubService
{
    Task<string> LockTennisClub(string clubId);
}