namespace ClubService.Domain.Event.Member;

public class MemberAccountUpdatedEvent(
    string firstName,
    string lastName,
    string email) : IMemberDomainEvent
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public string Email { get; } = email;
}