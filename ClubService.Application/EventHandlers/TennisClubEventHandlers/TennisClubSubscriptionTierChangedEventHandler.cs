using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TennisClubEventHandlers;

public class TennisClubSubscriptionTierChangedEventHandler(ITennisClubReadModelRepository tennisClubReadModelRepository)
    : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }
        
        var tennisClubSubscriptionTierChangedEvent = (TennisClubSubscriptionTierChangedEvent)domainEnvelope.EventData;
        var tennisClub = await tennisClubReadModelRepository.GetTennisClubById(domainEnvelope.EntityId);
        
        if (tennisClub != null)
        {
            tennisClub.ChangeSubscriptionTier(tennisClubSubscriptionTierChangedEvent.SubscriptionTierId);
            await tennisClubReadModelRepository.Update();
        }
        else
        {
            // TODO: Add logging
            Console.WriteLine($"Could not lock tennis club with id {domainEnvelope.EntityId}!");
        }
    }
    
    private bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED);
    }
}