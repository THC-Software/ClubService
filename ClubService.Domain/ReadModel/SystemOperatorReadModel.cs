using ClubService.Domain.Event.SystemOperator;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.ReadModel;

public class SystemOperatorReadModel
{
    private SystemOperatorReadModel()
    {
    }

    private SystemOperatorReadModel(SystemOperatorId systemOperatorId, string username)
    {
        SystemOperatorId = systemOperatorId;
        Username = username;
    }

    public SystemOperatorId SystemOperatorId { get; private set; } = null!;
    public string Username { get; private set; } = null!;

    public static SystemOperatorReadModel FromDomainEvent(SystemOperatorRegisteredEvent systemOperatorRegisteredEvent)
    {
        return new SystemOperatorReadModel(systemOperatorRegisteredEvent.SystemOperatorId,
            systemOperatorRegisteredEvent.Username);
    }
}