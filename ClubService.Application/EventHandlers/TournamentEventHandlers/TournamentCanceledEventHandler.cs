using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TournamentEventHandlers;

public class TournamentCanceledEventHandler(
    ITournamentReadModelRepository tournamentReadModelRepository,
    ILoggerService<TournamentCanceledEventHandler> loggerService)
    : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var tournamentReadModel = await tournamentReadModelRepository.GetTournamentById(domainEnvelope.EntityId);

        if (tournamentReadModel == null)
        {
            loggerService.LogTournamentNotFound(domainEnvelope.EntityId);
            return;
        }

        // TODO: Send email
        await tournamentReadModelRepository.Delete(tournamentReadModel);
        loggerService.LogTournamentCanceled(tournamentReadModel.TournamentId);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TOURNAMENT_CANCELED);
    }
}