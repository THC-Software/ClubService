using ClubService.Application.Api;

namespace ClubService.Application.Impl;

public class UpdateTennisClubService : IUpdateTennisClubService
{
    public string LockTennisClub(string clubId)
    {
        return clubId;
    }
}