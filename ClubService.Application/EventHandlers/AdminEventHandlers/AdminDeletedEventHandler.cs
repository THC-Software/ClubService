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
            loggerService.LogRejectEvent(domainEnvelope);
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var adminReadModel = await adminReadModelRepository.GetAdminById(domainEnvelope.EntityId);
        if (adminReadModel == null)
        {
            loggerService.LogAdminNotFound(domainEnvelope.EntityId);
            return;
        }

        await adminReadModelRepository.Delete(adminReadModel);
        loggerService.LogAdminDeleted(adminReadModel.AdminId.Id);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.ADMIN_DELETED);
    }
}