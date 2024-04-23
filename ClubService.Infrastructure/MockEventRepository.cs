using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Infrastructure;

public class MockEventRepository : IEventRepository
{
    private static List<DomainEnvelope<IDomainEvent>> EVENTS = new();

    public void Save<T>(DomainEnvelope<T> domainEnvelope) where T : IDomainEvent
    {
        EVENTS.Add(new DomainEnvelope<IDomainEvent>(
            domainEnvelope.EventId, 
            domainEnvelope.EntityId,
            domainEnvelope.EventType, 
            domainEnvelope.EntityType, 
            domainEnvelope.DomainEvent
        ));
    }
}