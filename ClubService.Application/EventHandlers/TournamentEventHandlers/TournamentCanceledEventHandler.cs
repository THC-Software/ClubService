using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TournamentEventHandlers;

public class TournamentCanceledEventHandler(ITournamentReadModelRepository tournamentReadModelRepository)
    : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }

        var tournamentReadModel = await tournamentReadModelRepository.GetTournamentById(domainEnvelope.EntityId);

        if (tournamentReadModel == null)
        {
            // TODO: Add logging
            Console.WriteLine($"Tournament with id '{domainEnvelope.EntityId}' not found!");
            return;
        }

        // TODO: Send email
        await tournamentReadModelRepository.Delete(tournamentReadModel);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TOURNAMENT_CANCELED);
    }
}