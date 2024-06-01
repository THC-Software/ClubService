using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.MemberEventHandlers;

public class MemberUnlockedEventHandler(IMemberReadModelRepository memberReadModelRepository) : IEventHandler
{
    public Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        throw new NotImplementedException();
    }
}