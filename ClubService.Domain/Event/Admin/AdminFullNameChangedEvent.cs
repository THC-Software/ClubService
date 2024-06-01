using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Event.Admin;

public class AdminFullNameChangedEvent(FullName name) : IAdminDomainEvent
{
    public FullName Name { get; } = name;
}