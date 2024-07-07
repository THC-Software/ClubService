using ClubService.Domain.Event;
using ClubService.Domain.Event.SystemOperator;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Model.Entity;

public class SystemOperator
{
    public SystemOperatorId SystemOperatorId { get; private set; } = null!;
    public string Username { get; private set; } = null!;

    public List<DomainEnvelope<ISystemOperatorDomainEvent>> ProcessSystemOperatorRegisterCommand(string username)
    {
        var systemOperatorRegisteredEvent =
            new SystemOperatorRegisteredEvent(new SystemOperatorId(Guid.NewGuid()), username);

        var domainEnvelope = new DomainEnvelope<ISystemOperatorDomainEvent>(
            Guid.NewGuid(),
            systemOperatorRegisteredEvent.SystemOperatorId.Id,
            EventType.SYSTEM_OPERATOR_REGISTERED,
            EntityType.SYSTEM_OPERATOR,
            DateTime.UtcNow,
            systemOperatorRegisteredEvent
        );

        return [domainEnvelope];
    }

    public void Apply(DomainEnvelope<ISystemOperatorDomainEvent> domainEnvelope)
    {
        switch (domainEnvelope.EventType)
        {
            case EventType.SYSTEM_OPERATOR_REGISTERED:
                Apply((SystemOperatorRegisteredEvent)domainEnvelope.EventData);
                break;
        }
    }

    private void Apply(SystemOperatorRegisteredEvent systemOperatorRegisteredEvent)
    {
        SystemOperatorId = systemOperatorRegisteredEvent.SystemOperatorId;
        Username = systemOperatorRegisteredEvent.Username;
    }
}