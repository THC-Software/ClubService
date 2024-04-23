using ClubService.Domain.Event;

namespace ClubService.Domain.Repository;

public interface IEventRepository
{
    void Save<T>(DomainEnvelope<T> domainEnvelope) where T : IDomainEvent;
}