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
    public async Task<Guid> RegisterTennisClub(TennisClubRegisterCommand tennisClubRegisterCommand)
    {
        var subscriptionTierId = new SubscriptionTierId(tennisClubRegisterCommand.SubscriptionTierId);
        var subscriptionTierDomainEvents =
            await eventRepository.GetEventsForEntity<ISubscriptionTierDomainEvent>(subscriptionTierId.Id);
        
        if (subscriptionTierDomainEvents.Count == 0)
        {
            throw new SubscriptionTierNotFoundException(subscriptionTierId.Id);
        }
        
        var tennisClub = new TennisClub();
        
        var domainEvents =
            tennisClub.ProcessTennisClubRegisterCommand(tennisClubRegisterCommand.Name,
                subscriptionTierId);
        var expectedEventCount = 0;
        
        foreach (var domainEvent in domainEvents)
        {
            tennisClub.Apply(domainEvent);
            expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
        }
        
        return tennisClub.TennisClubId.Id;
    }
}