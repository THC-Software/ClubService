using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class RegisterTennisClubService(IEventRepository eventRepository)
    : IRegisterTennisClubService
{
    public async Task<string> RegisterTennisClub(TennisClubRegisterCommand tennisClubRegisterCommand)
    {
        var subscriptionTierId = new Guid(tennisClubRegisterCommand.SubscriptionTierId);
        var subscriptionTierDomainEvents =
            eventRepository.GetEventsForEntity<ISubscriptionTierDomainEvent>(subscriptionTierId);
        
        if (subscriptionTierDomainEvents.Count == 0)
        {
            throw new SubscriptionTierNotFoundException($"Subscription Tier '{subscriptionTierId}' not found!");
        }
        
        var clubId = Guid.NewGuid();
        
        var tennisClub = TennisClub.Create(new TennisClubId(clubId));
        
        var tennisClubDomainEvents =
            tennisClub.ProcessTennisClubRegisterCommand(tennisClubRegisterCommand.Name,
                tennisClubRegisterCommand.SubscriptionTierId);
        
        foreach (var tennisClubDomainEvent in tennisClubDomainEvents)
        {
            tennisClub.Apply(tennisClubDomainEvent);
            await eventRepository.Save(tennisClubDomainEvent);
        }
        
        return clubId.ToString();
    }
}