using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class DeleteAdminService(IEventRepository eventRepository) : IDeleteAdminService
{
    public async Task<string> DeleteAdmin(string id)
    {
        var adminId = new AdminId(new Guid(id));
        var existingAdminDomainEvents = await eventRepository.GetEventsForEntity<IAdminDomainEvent>(adminId.Id);
        
        if (existingAdminDomainEvents.Count == 0)
        {
            throw new AdminNotFoundException(adminId.Id);
        }
        
        var initialEventCount = existingAdminDomainEvents.Count;
        
        var admin = new Admin();
        foreach (var domainEvent in existingAdminDomainEvents)
        {
            admin.Apply(domainEvent);
        }
        
        try
        {
            var domainEvents = admin.ProcessAdminDeleteCommand();
            
            await eventRepository.BeginTransactionAsync();
            
            foreach (var domainEvent in domainEvents)
            {
                admin.Apply(domainEvent);
                await eventRepository.Append(domainEvent);
            }
            
            existingAdminDomainEvents = await eventRepository.GetEventsForEntity<IAdminDomainEvent>(adminId.Id);
            
            if (existingAdminDomainEvents.Count != initialEventCount + domainEvents.Count)
            {
                throw new ConcurrencyException(
                    "Additional events added during processing of delete admin!");
            }
            
            await eventRepository.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            throw new ConflictException(ex.Message, ex);
        }
        catch (ConcurrencyException)
        {
            await eventRepository.RollbackTransactionAsync();
            throw;
        }
        
        return id;
    }
}