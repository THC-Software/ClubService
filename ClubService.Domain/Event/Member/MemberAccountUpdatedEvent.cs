namespace ClubService.Domain.Event.Member;

public class MemberAccountUpdatedEvent(
    Guid eventId,
    Guid entityId,
    EventType eventType,
    EntityType entityType,
    string firstName,
    string lastName,
    string email)
    : Event(eventId, entityId, eventType, entityType)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public string Email { get; } = email;
}