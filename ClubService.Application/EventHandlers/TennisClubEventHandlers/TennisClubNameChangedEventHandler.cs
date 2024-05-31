using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TennisClubEventHandlers;

public class TennisClubNameChangedEventHandler(ITennisClubReadModelRepository tennisClubReadModelRepository)
    : IEventHandler
{
    public Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        throw new NotImplementedException();
    }
}