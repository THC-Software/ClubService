using ClubService.Domain.Event;

namespace ClubService.Application.Api;

public interface IEventHandler
{
    Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope);
}