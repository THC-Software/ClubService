using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Infrastructure;

public class MockEventRepository : IEventRepository
{
    private static readonly List<DomainEnvelope<IDomainEvent>> EVENTS = new();

    public Task Save<T>(DomainEnvelope<T> domainEnvelope) where T : IDomainEvent
    {
        EVENTS.Add(new DomainEnvelope<IDomainEvent>(
            domainEnvelope.EventId,
            domainEnvelope.EntityId,
            domainEnvelope.EventType,
            domainEnvelope.EntityType,
            domainEnvelope.Timestamp,
            domainEnvelope.EventData
        ));
        return Task.FromResult("");
    }

    public List<DomainEnvelope<T>> GetEventsForEntity<T>(Guid entityId) where T : IDomainEvent
    {
        throw new NotImplementedException();
    }
}