using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class DeleteTennisClubService(IEventRepository eventRepository) : IDeleteTennisClubService
{
    public async Task<string> DeleteTennisClub(string clubId)
    {
        var tennisClubId = new TennisClubId(new Guid(clubId));
        var tennisClub = new TennisClub();
        
        var existingDomainEvents = eventRepository
            .GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id)
            .OrderBy(e => e.Timestamp)
            .ToList();
        
        if (existingDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException("No events found!");
        }
        
        var initialEventCount = existingDomainEvents.Count;
        
        foreach (var domainEvent in existingDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        //TODO: Delete all members and admins?
        try
        {
            var domainEvents = tennisClub.ProcessDeleteTennisClubCommand();
            
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
                throw new ConcurrencyException(
                    "Additional events added during processing delete tennis club!");
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
        
        return tennisClub.TennisClubId.Id.ToString();
    }
}