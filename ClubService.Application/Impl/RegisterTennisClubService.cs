using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class RegisterTennisClubService(IEventRepository eventRepository)
    : IRegisterTennisClubService
{
    public async Task<string> RegisterTennisClub(TennisClubRegisterCommand tennisClubRegisterCommand)
    {
        var subscriptionTierId = new Guid(tennisClubRegisterCommand.SubscriptionTierId);
        var subscriptionTierDomainEvents =
            await eventRepository.GetEventsForEntity<ISubscriptionTierDomainEvent>(subscriptionTierId);
        
        if (subscriptionTierDomainEvents.Count == 0)
        {
            throw new SubscriptionTierNotFoundException(subscriptionTierId);
        }
        
        var tennisClub = new TennisClub();
        
        var domainEvents =
            tennisClub.ProcessTennisClubRegisterCommand(tennisClubRegisterCommand.Name,
                tennisClubRegisterCommand.SubscriptionTierId);
        var expectedEventCount = 0;
        
        foreach (var domainEvent in domainEvents)
        {
            tennisClub.Apply(domainEvent);
            expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
        }
        
        return tennisClub.TennisClubId.Id.ToString();
    }
}