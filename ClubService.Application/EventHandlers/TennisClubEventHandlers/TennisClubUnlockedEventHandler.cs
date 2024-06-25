using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TennisClubEventHandlers;

public class TennisClubUnlockedEventHandler(
    ITennisClubReadModelRepository tennisClubReadModelRepository,
    ILoggerService<TennisClubUnlockedEventHandler> loggerService)
    : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            loggerService.LogRejectEvent(domainEnvelope);
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var tennisClubReadModel = await tennisClubReadModelRepository.GetTennisClubById(domainEnvelope.EntityId);

        if (tennisClubReadModel == null)
        {
            loggerService.LogTennisClubNotFound(domainEnvelope.EntityId);
            throw new TennisClubNotFoundException(domainEnvelope.EntityId);
        }

        tennisClubReadModel.Unlock();
        await tennisClubReadModelRepository.Update();
        loggerService.LogTennisClubUnlocked(tennisClubReadModel.TennisClubId.Id);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TENNIS_CLUB_UNLOCKED);
    }
}