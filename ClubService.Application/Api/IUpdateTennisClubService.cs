namespace ClubService.Application.Api;

public interface IUpdateTennisClubService
{
    Task<Guid> LockTennisClub(Guid id);
    Task<Guid> UnlockTennisClub(Guid id);
    Task<Guid> ChangeSubscriptionTier(Guid clubId, string subscriptionTierId);
    Task<Guid> ChangeName(Guid clubId, string name);
}