using System.Data;
using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class DeleteTennisClubService(IEventRepository eventRepository) : IDeleteTennisClubService
{
    public async Task<Guid> DeleteTennisClub(Guid id)
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
        
        //TODO: Delete all members and admins?
        try
        {
            var domainEvents = tennisClub.ProcessDeleteTennisClubCommand();
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
        
        return tennisClub.TennisClubId.Id;
    }
}