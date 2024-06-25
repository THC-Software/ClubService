using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TennisClubEventHandlers;

public class TennisClubNameChangedEventHandler(
    ITennisClubReadModelRepository tennisClubReadModelRepository,
    ILoggerService<TennisClubNameChangedEventHandler> loggerService)
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

        var tennisClubNameChangedEvent = (TennisClubNameChangedEvent)domainEnvelope.EventData;
        var tennisClubReadModel = await tennisClubReadModelRepository.GetTennisClubById(domainEnvelope.EntityId);

        if (tennisClubReadModel == null)
        {
            loggerService.LogTennisClubNotFound(domainEnvelope.EntityId);
            throw new TennisClubNotFoundException(domainEnvelope.EntityId);
        }

        tennisClubReadModel.ChangeName(tennisClubNameChangedEvent.Name);
        await tennisClubReadModelRepository.Update();
        loggerService.LogTennisClubNameChanged(tennisClubReadModel.TennisClubId.Id);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TENNIS_CLUB_NAME_CHANGED);
    }
}