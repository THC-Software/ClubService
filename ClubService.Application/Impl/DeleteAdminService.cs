using System.Data;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.Impl;

public class DeleteAdminService(
    IEventRepository eventRepository,
    IEventStoreTransactionManager eventStoreTransactionManager) : IDeleteAdminService
{
    public async Task<string> DeleteAdmin(string id)
    {
        var adminId = new AdminId(new Guid(id));
        var existingAdminDomainEvents = await eventRepository.GetEventsForEntity<IAdminDomainEvent>(adminId.Id);
        
        if (existingAdminDomainEvents.Count == 0)
        {
            throw new AdminNotFoundException(adminId.Id);
        }
        
        var admin = new Admin();
        foreach (var domainEvent in existingAdminDomainEvents)
        {
            admin.Apply(domainEvent);
        }
        
        try
        {
            var domainEvents = admin.ProcessAdminDeleteCommand();
            var expectedEventCount = existingAdminDomainEvents.Count;
            
            await eventStoreTransactionManager.TransactionScope(async () =>
            {
                foreach (var domainEvent in domainEvents)
                {
                    admin.Apply(domainEvent);
                    expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            throw new ConflictException(ex.Message, ex);
        }
        
        return id;
    }
}