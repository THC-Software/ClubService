﻿using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.AdminEventHandlers;

public class AdminFullNameChangedEventHandler(IAdminReadModelRepository adminReadModelRepository) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }
        
        var adminNameChangedEvent = (AdminFullNameChangedEvent)domainEnvelope.EventData;
        var adminReadModel = await adminReadModelRepository.GetAdminById(domainEnvelope.EntityId);
        
        if (adminReadModel == null)
        {
            // TODO: Add logging
            Console.WriteLine($"Admin with id {domainEnvelope.EntityId} not found!");
            return;
        }
        
        adminReadModel.ChangeFullName(adminNameChangedEvent.Name);
        await adminReadModelRepository.Update();
    }
    
    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.ADMIN_FULL_NAME_CHANGED);
    }
}