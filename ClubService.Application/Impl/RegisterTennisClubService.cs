using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class RegisterTennisClubService(IEventRepository eventRepository) : IRegisterTennisClubService
{
    public Task<string> RegisterTennisClub(TennisClubRegisterCommand tennisClubRegisterCommand)
    {
        // TODO: Use Repository
        var clubId = Guid.NewGuid();
        
        var tennisClub = TennisClub.Create(new TennisClubId(clubId));
        
        List<DomainEnvelope<ITennisClubDomainEvent>> tennisClubDomainEvents = 
            tennisClub.ProcessTennisClubRegisterCommand(tennisClubRegisterCommand.Name, tennisClubRegisterCommand.SubscriptionTier);
        
        foreach (var tennisClubDomainEvent in tennisClubDomainEvents)
        {
            tennisClub.Apply(tennisClubDomainEvent);
            eventRepository.Save(tennisClubDomainEvent);
            // TODO: Publish event
        }
        
        return Task.FromResult(clubId.ToString());
    }
}