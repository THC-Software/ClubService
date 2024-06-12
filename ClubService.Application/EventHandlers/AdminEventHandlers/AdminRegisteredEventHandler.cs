using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.AdminEventHandlers;

public class AdminRegisteredEventHandler(
    IAdminReadModelRepository adminReadModelRepository,
    ILoggerService<AdminRegisteredEventHandler> loggerService) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var adminRegisteredEvent = (AdminRegisteredEvent)domainEnvelope.EventData;
        var adminReadModel = AdminReadModel.FromDomainEvent(adminRegisteredEvent);

        await adminReadModelRepository.Add(adminReadModel);
        loggerService.LogAdminRegistered(adminReadModel.AdminId.Id);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.ADMIN_REGISTERED);
    }
}