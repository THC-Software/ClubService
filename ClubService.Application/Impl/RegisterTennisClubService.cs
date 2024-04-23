using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Application.Impl;

public class RegisterTennisClubService : IRegisterTennisClubService
{
    public Task<string> CreateTennisClub(TennisClubRegisterCommand tennisClubRegisterCommand)
    {
        // TODO: Use Repository
        Guid clubId = Guid.NewGuid();

        // Create empty tennis club with id
        var tennisClub = TennisClub.Create(new TennisClubId(clubId));

        // Call process with command on tennis club object
        List<DomainEnvelope<ITennisClubDomainEvent>> tennisClubDomainEvents = 
            tennisClub.ProcessTennisClubRegisterCommand(tennisClubRegisterCommand.Name, tennisClubRegisterCommand.SubscriptionTier);
        
        // Apply events on tennis club object and store event in database
        foreach (var tennisClubDomainEvent in tennisClubDomainEvents)
        {
            tennisClub.Apply(tennisClubDomainEvent);
            // TODO: Save and publish event
        }
        
        return Task.FromResult(clubId.ToString());
    }
}