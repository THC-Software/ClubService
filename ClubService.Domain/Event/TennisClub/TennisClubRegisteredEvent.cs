namespace ClubService.Domain.Event.TennisClub;

public class TennisClubRegisteredEvent(
    Guid eventId,
    Guid entityId,
    EventType eventType,
    EntityType entityType,
    Model.Entity.TennisClub tennisClub)
    : Event(eventId, entityId, eventType, entityType)
{
    public Model.Entity.TennisClub TennisClub { get; } = tennisClub;
}