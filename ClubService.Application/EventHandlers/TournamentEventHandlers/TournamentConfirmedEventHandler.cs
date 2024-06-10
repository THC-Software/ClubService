using ClubService.Application.Api;
using ClubService.Domain.Event;

namespace ClubService.Application.EventHandlers.TournamentEventHandlers;

public class TournamentConfirmedEventHandler : IEventHandler
{
    public Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        throw new NotImplementedException();
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TOURNAMENT_CONFIRMED);
    }
}