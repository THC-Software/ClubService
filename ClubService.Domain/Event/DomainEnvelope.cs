using ClubService.Domain.Event;

namespace ClubService.Domain.Event;

public class DomainEnvelope<T>(
    Guid eventId,
    Guid entityId,
    EventType eventType,
    EntityType entityType,
    T domainEvent) where T : IDomainEvent
{
    public long Id { get; }
    public Guid EventId { get; } = eventId;
    public Guid EntityId { get; } = entityId;
    public EventType EventType { get; } = eventType;
    public EntityType EntityType { get; } = entityType;
    public T DomainEvent { get; } = domainEvent;
}