using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TennisClubEventHandlers;

public class TennisClubLockedEventHandler(ITennisClubReadModelRepository tennisClubReadModelRepository) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }
        
        var tennisClub = await tennisClubReadModelRepository.GetTennisClubById(domainEnvelope.EntityId);
        
        if (tennisClub == null)
        {
            // TODO: Add logging
            Console.WriteLine($"Tennis club with id {domainEnvelope.EntityId} not found!");
            return;
        }
        
        tennisClub.Lock();
        await tennisClubReadModelRepository.Update();
    }
    
    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TENNIS_CLUB_LOCKED);
    }
}