namespace ClubService.Domain.Event.Member;

public class MemberAccountCreatedEvent(
    Guid eventId,
    Guid entityId,
    EventType eventType,
    EntityType entityType,
    Model.Entity.Member member)
    : Event(eventId, entityId, eventType, entityType)
{
    public Model.Entity.Member Member { get; } = member;
}