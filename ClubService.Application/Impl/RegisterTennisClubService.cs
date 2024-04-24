using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class RegisterTennisClubService(IEventRepository eventRepository)
    : IRegisterTennisClubService
{
    public Task<string> RegisterTennisClub(TennisClubRegisterCommand tennisClubRegisterCommand)
    {
        // TODO: Use Repository
        var clubId = Guid.NewGuid();

        var tennisClub = TennisClub.Create(new TennisClubId(clubId));

        var tennisClubDomainEvents =
            tennisClub.ProcessTennisClubRegisterCommand(tennisClubRegisterCommand.Name,
                tennisClubRegisterCommand.SubscriptionTier);

        foreach (var tennisClubDomainEvent in tennisClubDomainEvents)
        {
            tennisClub.Apply(tennisClubDomainEvent);
            eventRepository.Save(tennisClubDomainEvent);
        }

        return Task.FromResult(clubId.ToString());
    }
}