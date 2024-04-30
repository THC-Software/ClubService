using ClubService.Application.Api;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class UpdateTennisClubService(IEventRepository eventRepository) : IUpdateTennisClubService
{
    public string LockTennisClub(string clubId)
    {
        // Create blank tennis club with given id
        var tennisClubId = new TennisClubId(new Guid(clubId));
        var tennisClub = TennisClub.Create(tennisClubId);

        // Get events for tennis club with tennisClubId
        var domainEvents = eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);

        if (domainEvents.Count == 0)
        {
            // TODO: Throw NotFoundException
            throw new ArgumentException();
        }

        // Apply all events on tennisClub
        foreach (var domainEvent in domainEvents)
        {
            tennisClub.Apply(domainEvent);
        }

        // Process LockTennisClub
        // Apply returned event
        // Check if there was no new event since the events were loaded

        return clubId;
    }
}