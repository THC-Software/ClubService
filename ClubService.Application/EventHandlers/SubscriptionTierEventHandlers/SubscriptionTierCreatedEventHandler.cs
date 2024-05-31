using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.SubscriptionTierEventHandlers;

public class SubscriptionTierCreatedEventHandler(
    ISubscriptionTierReadModelRepository subscriptionTierReadModelRepository) : IEventHandler
{
    public void Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }
        
        var subscriptionTierReadModel =
            SubscriptionTierReadModel.FromDomainEvent((SubscriptionTierCreatedEvent)domainEnvelope.EventData);
        subscriptionTierReadModelRepository.Add(subscriptionTierReadModel);
    }
    
    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.SUBSCRIPTION_TIER_CREATED);
    }
}