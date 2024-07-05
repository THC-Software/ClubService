using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.SystemOperator;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.SystemOperatorEventHandlers;

public class SystemOperatorRegisteredEventHandler(
    ISystemOperatorReadModelRepository systemOperatorReadModelRepository,
    ILoggerService<SystemOperatorRegisteredEventHandler> loggerService) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var systemOperatorReadModel =
            SystemOperatorReadModel.FromDomainEvent((SystemOperatorRegisteredEvent)domainEnvelope.EventData);
        await systemOperatorReadModelRepository.Add(systemOperatorReadModel);

        loggerService.LogSystemOperatorRegistered(systemOperatorReadModel.SystemOperatorId.Id);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.SYSTEM_OPERATOR_REGISTERED);
    }
}