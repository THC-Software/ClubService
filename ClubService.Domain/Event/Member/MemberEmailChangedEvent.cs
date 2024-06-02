namespace ClubService.Domain.Event.Member;

public class MemberEmailChangedEvent(string email) : IMemberDomainEvent
{
    public string Email { get; } = email;
}