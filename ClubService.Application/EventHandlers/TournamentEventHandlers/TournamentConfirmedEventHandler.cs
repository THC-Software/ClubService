using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Tournament;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TournamentEventHandlers;

public class TournamentConfirmedEventHandler(
    ITennisClubReadModelRepository tennisClubReadModelRepository,
    ITournamentReadModelRepository tournamentReadModelRepository)
    : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }

        var tournamentConfirmedEvent = (TournamentConfirmedEvent)domainEnvelope.EventData;
        var tennisClubReadModel =
            await tennisClubReadModelRepository.GetTennisClubById(tournamentConfirmedEvent.ClubId);

        if (tennisClubReadModel == null)
        {
            // TODO: Add logging
            Console.WriteLine($"Tennis Club with id '{tournamentConfirmedEvent.ClubId}' not found!");
            return;
        }

        // TODO: Send mail
        var tournamentReadModel = TournamentReadModel.FromDomainEvent(tournamentConfirmedEvent);
        await tournamentReadModelRepository.Add(tournamentReadModel);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TOURNAMENT_CONFIRMED);
    }
}