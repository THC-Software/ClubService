using System.Data;
using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.Impl;

public class UpdateTennisClubService(
    IEventRepository eventRepository,
    IEventStoreTransactionManager eventStoreTransactionManager) : IUpdateTennisClubService
{
    public async Task<string> LockTennisClub(string clubId)
    {
        var tennisClubId = new TennisClubId(new Guid(clubId));
        var tennisClub = new TennisClub();
        
        var existingTennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
        
        if (existingTennisClubDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException(tennisClubId.Id);
        }
        
        foreach (var domainEvent in existingTennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        try
        {
            var domainEvents = tennisClub.ProcessTennisClubLockCommand();
            var expectedEventCount = existingTennisClubDomainEvents.Count;
            
            await eventStoreTransactionManager.TransactionScope(async () =>
            {
                foreach (var domainEvent in domainEvents)
                {
                    tennisClub.Apply(domainEvent);
                    expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            throw new ConflictException(ex.Message, ex);
        }
        
        return clubId;
    }
    
    public async Task<string> UnlockTennisClub(string clubId)
    {
        var tennisClubId = new TennisClubId(new Guid(clubId));
        var tennisClub = new TennisClub();
        
        var existingTennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
        
        if (existingTennisClubDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException(tennisClubId.Id);
        }
        
        foreach (var domainEvent in existingTennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        try
        {
            var domainEvents = tennisClub.ProcessTennisClubUnlockCommand();
            var expectedEventCount = existingTennisClubDomainEvents.Count;
            
            foreach (var domainEvent in domainEvents)
            {
                tennisClub.Apply(domainEvent);
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
        
        return clubId;
    }
    
    public async Task<string> ChangeSubscriptionTier(string clubId, string subscriptionTierId)
    {
        var tennisClubId = new TennisClubId(new Guid(clubId));
        var tennisClub = new TennisClub();
        
        var existingTennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
        
        if (existingTennisClubDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException(tennisClubId.Id);
        }
        
        foreach (var domainEvent in existingTennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        try
        {
            var domainEvents = tennisClub.ProcessTennisClubChangeSubscriptionTierCommand(subscriptionTierId);
            var expectedEventCount = existingTennisClubDomainEvents.Count;
            
            foreach (var domainEvent in domainEvents)
            {
                tennisClub.Apply(domainEvent);
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
        
        return clubId;
    }
    
    public async Task<string> ChangeName(string clubId, string name)
    {
        var tennisClubId = new TennisClubId(new Guid(clubId));
        var tennisClub = new TennisClub();
        
        var existingTennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
        
        if (existingTennisClubDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException(tennisClubId.Id);
        }
        
        foreach (var domainEvent in existingTennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        try
        {
            var domainEvents = tennisClub.ProcessTennisClubChangeNameCommand(name);
            var expectedEventCount = existingTennisClubDomainEvents.Count;
            
            foreach (var domainEvent in domainEvents)
            {
                tennisClub.Apply(domainEvent);
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
        
        return clubId;
    }
}