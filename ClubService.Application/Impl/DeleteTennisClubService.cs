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
        
        var existingDomainEvents = await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
        
        if (existingDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException(tennisClubId.Id);
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
                await eventRepository.Append(domainEvent);
            }
            
            existingDomainEvents = await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
            
            if (existingDomainEvents.Count != initialEventCount + domainEvents.Count)
            {
                throw new ConcurrencyException(
                    "Additional events added during processing of delete tennis club!");
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