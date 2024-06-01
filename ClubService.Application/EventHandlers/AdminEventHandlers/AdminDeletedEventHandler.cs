using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.AdminEventHandlers;

public class AdminDeletedEventHandler(IAdminReadModelRepository adminReadModelRepository) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }
        
        var adminReadModel = await adminReadModelRepository.GetAdminById(domainEnvelope.EntityId);
        if (adminReadModel != null)
        {
            await adminReadModelRepository.Delete(adminReadModel);
        }
        else
        {
            // TODO: Add logging
            Console.WriteLine($"Admin with id {domainEnvelope.EntityId} not found!");
        }
    }
    
    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.ADMIN_DELETED);
    }
}