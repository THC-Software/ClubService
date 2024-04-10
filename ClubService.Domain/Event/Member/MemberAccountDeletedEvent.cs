namespace ClubService.Domain.Event.Member;

public class MemberAccountDeletedEvent(
    Guid eventId, 
    Guid entityId, 
    EventType eventType, 
    EntityType entityType)
    : Event(eventId, entityId, eventType, entityType)
{
}