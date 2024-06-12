using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TennisClubEventHandlers;

public class TennisClubLockedEventHandler(
    ITennisClubReadModelRepository tennisClubReadModelRepository,
    ILoggerService<TennisClubLockedEventHandler> loggerService) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var tennisClubReadModel = await tennisClubReadModelRepository.GetTennisClubById(domainEnvelope.EntityId);

        if (tennisClubReadModel == null)
        {
            loggerService.LogTennisClubNotFound(domainEnvelope.EntityId);
            return;
        }

        tennisClubReadModel.Lock();
        await tennisClubReadModelRepository.Update();
        loggerService.LogTennisClubLocked(tennisClubReadModel.TennisClubId.Id);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TENNIS_CLUB_LOCKED);
    }
}