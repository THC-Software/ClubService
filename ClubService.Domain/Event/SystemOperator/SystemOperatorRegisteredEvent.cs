using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Event.SystemOperator;

public class SystemOperatorRegisteredEvent(SystemOperatorId systemOperatorId, string username) : ISystemOperatorDomainEvent
{
    public SystemOperatorId SystemOperatorId { get; } = systemOperatorId;
    public string Username { get; } = username;
}