using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.SubscriptionTierEventHandlers;

public class SubscriptionTierCreatedEventHandler(
    ISubscriptionTierReadModelRepository subscriptionTierReadModelRepository,
    ILoggerService<SubscriptionTierCreatedEventHandler> loggerService) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            loggerService.LogRejectEvent(domainEnvelope);
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var subscriptionTierCreatedEvent = (SubscriptionTierCreatedEvent)domainEnvelope.EventData;
        var subscriptionTierReadModel = SubscriptionTierReadModel.FromDomainEvent(subscriptionTierCreatedEvent);

        await subscriptionTierReadModelRepository.Add(subscriptionTierReadModel);
        loggerService.LogSubscriptionTierCreated(subscriptionTierReadModel.SubscriptionTierId.Id);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.SUBSCRIPTION_TIER_CREATED);
    }
}