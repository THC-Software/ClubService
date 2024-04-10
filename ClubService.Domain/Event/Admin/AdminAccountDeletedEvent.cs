namespace ClubService.Domain.Event.Admin;

public class AdminAccountDeletedEvent(
    Guid eventId, 
    Guid entityId, 
    EventType eventType, 
    EntityType entityType)
    : Event(eventId, entityId, eventType, entityType)
{
}