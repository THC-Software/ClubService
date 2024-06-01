using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Event.Member;

public class MemberFullNameChangedEvent(FullName name) : IMemberDomainEvent
{
    public FullName Name { get; } = name;
}