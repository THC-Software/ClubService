using ClubService.Application.Api;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class UpdateTennisClubService(IEventRepository eventRepository) : IUpdateTennisClubService
{
    // TODO: Ensure that there were no other events added during processing locking the tennis club
    public async Task<string> LockTennisClub(string clubId)
    {
        var tennisClubId = new TennisClubId(new Guid(clubId));
        var tennisClub = TennisClub.Create(tennisClubId);

        var existingDomainEvents =
            eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);

        if (existingDomainEvents.Count == 0)
        {
            // TODO: Throw NotFoundException
            throw new ArgumentException();
        }

        foreach (var domainEvent in existingDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }

        var domainEvents = tennisClub.ProcessTennisClubLockCommand();

        foreach (var domainEvent in domainEvents)
        {
            tennisClub.Apply(domainEvent);
            await eventRepository.Save(domainEvent);
        }

        return clubId;
    }
}