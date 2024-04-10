namespace ClubService.Domain.Event.TennisClub;

public class TennisClubUnlockedEvent(
    Guid eventId, 
    Guid entityId, 
    EventType eventType, 
    EntityType entityType)
    : Event(eventId, entityId, eventType, entityType)
{
}