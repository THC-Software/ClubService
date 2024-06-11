using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TournamentEventHandlers;

public class TournamentCanceledEventHandler(ITournamentReadModelRepository tournamentReadModelRepository)
    : IEventHandler
{
    public Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        throw new NotImplementedException();
    }
}