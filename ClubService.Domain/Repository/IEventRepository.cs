using ClubService.Domain.Event;

namespace ClubService.Domain.Repository;

public interface IEventRepository
{
    Task Append<T>(DomainEnvelope<T> domainEnvelope) where T : IDomainEvent;
    Task<List<DomainEnvelope<T>>> GetEventsForEntity<T>(Guid entityId) where T : IDomainEvent;
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}