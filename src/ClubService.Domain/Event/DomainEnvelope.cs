namespace ClubService.Domain.Event;

public class DomainEnvelope<T>(
    Guid eventId,
    Guid entityId,
    EventType eventType,
    EntityType entityType,
    DateTime timestamp,
    T eventData) where T : IDomainEvent
{
    public Guid EventId { get; } = eventId;
    public Guid EntityId { get; } = entityId;
    public EventType EventType { get; } = eventType;
    public EntityType EntityType { get; } = entityType;
    public DateTime Timestamp { get; } = timestamp;
    public T EventData { get; } = eventData;
}