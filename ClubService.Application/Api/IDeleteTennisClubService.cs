namespace ClubService.Application.Api;

public interface IDeleteTennisClubService
{
    Task<string> DeleteTennisClub(string clubId);
}