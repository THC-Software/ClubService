using System.Data;
using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class UpdateAdminService(IEventRepository eventRepository) : IUpdateAdminService
{
    public async Task<Guid> ChangeFullName(Guid id, string firstName, string lastName)
    {
        var existingAdminDomainEvents = await eventRepository.GetEventsForEntity<IAdminDomainEvent>(id);
        
        if (existingAdminDomainEvents.Count == 0)
        {
            throw new AdminNotFoundException(id);
        }
        
        var admin = new Admin();
        
        foreach (var domainEvent in existingAdminDomainEvents)
        {
            admin.Apply(domainEvent);
        }
        
        try
        {
            var domainEvents = admin.ProcessAdminChangeFullNameCommand(new FullName(firstName, lastName));
            var expectedEventCount = existingAdminDomainEvents.Count;
            
            foreach (var domainEvent in domainEvents)
            {
                admin.Apply(domainEvent);
                expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
            }
        }
        catch (InvalidOperationException ex)
        {
            throw new ConflictException(ex.Message, ex);
        }
        catch (DataException ex)
        {
            throw new ConcurrencyException(ex.Message, ex);
        }
        
        return id;
    }
}