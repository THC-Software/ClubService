using ClubService.Domain.Event;

namespace ClubService.Domain.Repository;

public interface IEventPublisher
{
    Task PublishEvent<T>(DomainEnvelope<T> domainEnvelope) where T : IDomainEvent;
}