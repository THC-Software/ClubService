using ClubService.Domain.Event;

namespace ClubService.Domain.Repository;

public interface IEventRepository
{
    Task Save<T>(DomainEnvelope<T> domainEnvelope) where T : IDomainEvent;
    List<DomainEnvelope<IDomainEvent>> GetEventsForEntity<T>(Guid entityId) where T : IDomainEvent;
}