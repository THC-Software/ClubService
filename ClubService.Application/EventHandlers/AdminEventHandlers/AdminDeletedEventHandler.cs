using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.AdminEventHandlers;

public class AdminDeletedEventHandler(
    IAdminReadModelRepository adminReadModelRepository,
    ILoggerService<AdminDeletedEventHandler> loggerService) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }

        loggerService.LogAdminDeletedEventHandler(domainEnvelope);

        var adminReadModel = await adminReadModelRepository.GetAdminById(domainEnvelope.EntityId);
        if (adminReadModel == null)
        {
            loggerService.LogAdminNotFound(domainEnvelope.EntityId);
            return;
        }

        loggerService.LogAdminDeleted(domainEnvelope.EntityId);
        await adminReadModelRepository.Delete(adminReadModel);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.ADMIN_DELETED);
    }
}