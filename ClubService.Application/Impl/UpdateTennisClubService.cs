using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class UpdateTennisClubService(IEventRepository eventRepository) : IUpdateTennisClubService
{
    public async Task<string> LockTennisClub(string clubId)
    {
        var tennisClubId = new TennisClubId(new Guid(clubId));
        var tennisClub = new TennisClub();
        
        var existingDomainEvents = await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
        
        if (existingDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException("No events found!");
        }
        
        var initialEventCount = existingDomainEvents.Count;
        
        foreach (var domainEvent in existingDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        try
        {
            var domainEvents = tennisClub.ProcessTennisClubLockCommand();
            
            await eventRepository.BeginTransactionAsync();
            
            foreach (var domainEvent in domainEvents)
            {
                tennisClub.Apply(domainEvent);
                await eventRepository.Append(domainEvent);
            }
            
            existingDomainEvents = await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
            
            if (existingDomainEvents.Count != initialEventCount + domainEvents.Count)
            {
                throw new ConcurrencyException(
                    "Additional events added during processing locking the tennis club!");
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
        
        return clubId;
    }
    
    public async Task<string> UnlockTennisClub(string clubId)
    {
        var tennisClubId = new TennisClubId(new Guid(clubId));
        var tennisClub = new TennisClub();
        
        var existingDomainEvents = await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
        
        if (existingDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException("No events found!");
        }
        
        var initialEventCount = existingDomainEvents.Count;
        
        foreach (var domainEvent in existingDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        try
        {
            var domainEvents = tennisClub.ProcessTennisClubUnlockCommand();
            
            await eventRepository.BeginTransactionAsync();
            
            foreach (var domainEvent in domainEvents)
            {
                tennisClub.Apply(domainEvent);
                await eventRepository.Append(domainEvent);
            }
            
            existingDomainEvents = await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
            
            if (existingDomainEvents.Count != initialEventCount + domainEvents.Count)
            {
                throw new ConcurrencyException(
                    "Additional events added during processing unlocking the tennis club!");
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
        
        return clubId;
    }
    
    public async Task<string> ChangeSubscriptionTier(string clubId, string subscriptionTierId)
    {
        var tennisClubId = new TennisClubId(new Guid(clubId));
        var tennisClub = new TennisClub();
        
        var existingDomainEvents = await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
        
        if (existingDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException("No events found!");
        }
        
        var initialEventCount = existingDomainEvents.Count;
        
        foreach (var domainEvent in existingDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        try
        {
            var domainEvents = tennisClub.ProcessTennisClubChangeSubscriptionTierCommand(subscriptionTierId);
            
            await eventRepository.BeginTransactionAsync();
            
            foreach (var domainEvent in domainEvents)
            {
                tennisClub.Apply(domainEvent);
                await eventRepository.Append(domainEvent);
            }
            
            existingDomainEvents = await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
            
            if (existingDomainEvents.Count != initialEventCount + domainEvents.Count)
            {
                throw new ConcurrencyException(
                    "Additional events added during processing changing subscription tier!");
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
        
        return clubId;
    }
    
    public async Task<string> ChangeName(string clubId, string name)
    {
        var tennisClubId = new TennisClubId(new Guid(clubId));
        var tennisClub = new TennisClub();
        
        var existingDomainEvents = await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
        
        if (existingDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException("No events found!");
        }
        
        var initialEventCount = existingDomainEvents.Count;
        
        foreach (var domainEvent in existingDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        try
        {
            var domainEvents = tennisClub.ProcessTennisClubChangeNameCommand(name);
            
            await eventRepository.BeginTransactionAsync();
            
            foreach (var domainEvent in domainEvents)
            {
                tennisClub.Apply(domainEvent);
                await eventRepository.Append(domainEvent);
            }
            
            existingDomainEvents = await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
            
            if (existingDomainEvents.Count != initialEventCount + domainEvents.Count)
            {
                throw new ConcurrencyException(
                    "Additional events added during processing changing name!");
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
        
        return clubId;
    }
}