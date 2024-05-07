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
        var tennisClub = TennisClub.Create(tennisClubId);
        
        var existingDomainEvents =
            eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
        
        if (existingDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException("No events found!");
        }
        
        var initialEventCount = existingDomainEvents.Count;
        
        foreach (var domainEvent in existingDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        var domainEvents = tennisClub.ProcessTennisClubLockCommand();
        
        try
        {
            await eventRepository.BeginTransactionAsync();
            
            foreach (var domainEvent in domainEvents)
            {
                tennisClub.Apply(domainEvent);
                await eventRepository.Save(domainEvent);
            }
            
            existingDomainEvents =
                eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
            
            if (existingDomainEvents.Count != initialEventCount + domainEvents.Count)
            {
                throw new InvalidOperationException(
                    "Additional events added during processing locking the tennis club!");
            }
            
            await eventRepository.CommitTransactionAsync();
        }
        catch (InvalidOperationException)
        {
            await eventRepository.RollbackTransactionAsync();
            throw;
        }
        
        return clubId;
    }
    
    public Task<string> UnlockTennisClub(string clubId)
    {
        throw new NotImplementedException();
    }
}