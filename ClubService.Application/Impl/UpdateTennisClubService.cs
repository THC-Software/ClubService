using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Domain.Event.SubscriptionTier;
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
    public async Task<Guid> LockTennisClub(Guid id)
    {
        var tennisClubId = new TennisClubId(id);
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
        
        return id;
    }
    
    public async Task<Guid> UnlockTennisClub(Guid id)
    {
        var tennisClubId = new TennisClubId(id);
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
        
        return id;
    }
    
    // TODO: Remove after update is refactored
    public async Task<Guid> ChangeSubscriptionTier(Guid clubId, Guid subscriptionTierGuid)
    {
        var subscriptionTierId = new SubscriptionTierId(subscriptionTierGuid);
        var tennisClubId = new TennisClubId(clubId);
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
    
    // TODO: Remove after update is refactored
    public async Task<Guid> ChangeName(Guid clubId, string name)
    {
        var tennisClubId = new TennisClubId(clubId);
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
    
    public async Task<Guid> UpdateTennisClub(Guid id, TennisClubUpdateCommand tennisClubUpdateCommand)
    {
        // TODO: Check that not both properties of tennisClubUpdateCommand are null
        var tennisClubId = new TennisClubId(id);
        
        var existingTennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
        
        if (existingTennisClubDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException(tennisClubId.Id);
        }
        
        SubscriptionTierId? subscriptionTierId = null;
        if (tennisClubUpdateCommand.SubscriptionTierId != null)
        {
            subscriptionTierId = new SubscriptionTierId((Guid)tennisClubUpdateCommand.SubscriptionTierId);
            
            var existingSubscriptionTierDomainEvents =
                await eventRepository.GetEventsForEntity<ISubscriptionTierDomainEvent>(subscriptionTierId.Id);
            
            if (existingSubscriptionTierDomainEvents.Count == 0)
            {
                throw new TennisClubNotFoundException(tennisClubId.Id);
            }
        }
        
        var tennisClub = new TennisClub();
        foreach (var domainEvent in existingTennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        try
        {
            var domainEvents =
                tennisClub.ProcessTennisClubUpdateCommand(tennisClubUpdateCommand.Name, subscriptionTierId);
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
        
        return id;
    }
}