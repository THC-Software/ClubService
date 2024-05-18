namespace ClubService.Application.Api;

public interface IUpdateTennisClubService
{
    Task<string> LockTennisClub(string clubId);
    Task<string> UnlockTennisClub(string clubId);
    Task<string> ChangeSubscriptionTier(string clubId, string subscriptionTierId);
}