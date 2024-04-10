namespace ClubService.Domain.Event.Admin;

public class AdminAccountCreatedEvent(
    Guid eventId,
    Guid entityId,
    EventType eventType,
    EntityType entityType,
    Model.Entity.Admin admin) : Event(eventId, entityId, eventType, entityType)
{
    public Model.Entity.Admin Admin { get; } = admin;
}