using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TennisClubEventHandlers;

public class TennisClubSubscriptionTierChangedEventHandler(
    ITennisClubReadModelRepository tennisClubReadModelRepository,
    ILoggerService<TennisClubSubscriptionTierChangedEventHandler> loggerService)
    : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var tennisClubSubscriptionTierChangedEvent = (TennisClubSubscriptionTierChangedEvent)domainEnvelope.EventData;
        var tennisClubReadModel = await tennisClubReadModelRepository.GetTennisClubById(domainEnvelope.EntityId);

        if (tennisClubReadModel == null)
        {
            loggerService.LogTennisClubNotFound(domainEnvelope.EntityId);
            return;
        }

        tennisClubReadModel.ChangeSubscriptionTier(tennisClubSubscriptionTierChangedEvent.SubscriptionTierId);
        await tennisClubReadModelRepository.Update();
        loggerService.LogTennisClubSubscriptionTierChanged(tennisClubReadModel.TennisClubId.Id);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED);
    }
}