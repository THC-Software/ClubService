using ClubService.Domain.Event;

namespace ClubService.Application.Api;

public interface IEventHandler
{
    void Handle(DomainEnvelope<IDomainEvent> domainEnvelope);
}