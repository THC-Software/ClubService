namespace ClubService.Domain.Event;

public abstract class Event(Guid eventId, Guid entityId, EventType eventType, EntityType entityType)
{
    public Guid EventId { get; } = eventId;
    public Guid EntityId { get; } = entityId;
    public EventType EventType { get; } = eventType;
    public EntityType EntityType { get; } = entityType;
}