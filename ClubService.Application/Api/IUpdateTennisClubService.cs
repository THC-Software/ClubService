using ClubService.Application.Commands;

namespace ClubService.Application.Api;

public interface IUpdateTennisClubService
{
    Task<Guid> LockTennisClub(Guid id);
    Task<Guid> UnlockTennisClub(Guid id);
    Task<Guid> ChangeSubscriptionTier(Guid clubId, Guid subscriptionTierGuid);
    Task<Guid> ChangeName(Guid clubId, string name);
    Task<Guid> UpdateTennisClub(Guid id, TennisClubUpdateCommand tennisClubUpdateCommand);
}