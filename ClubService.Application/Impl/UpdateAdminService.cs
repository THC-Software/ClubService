using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.Impl;

public class UpdateAdminService(
    IEventRepository eventRepository,
    IEventStoreTransactionManager eventStoreTransactionManager) : IUpdateAdminService
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
        
        var tennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(admin.TennisClubId.Id);
        
        var tennisClub = new TennisClub();
        foreach (var domainEvent in tennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        switch (tennisClub.Status)
        {
            case TennisClubStatus.ACTIVE:
                try
                {
                    var domainEvents = admin.ProcessAdminChangeFullNameCommand(new FullName(firstName, lastName));
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
            case TennisClubStatus.LOCKED:
                throw new ConflictException("Tennis club is locked!");
            case TennisClubStatus.DELETED:
                throw new ConflictException("Tennis club already deleted!");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}