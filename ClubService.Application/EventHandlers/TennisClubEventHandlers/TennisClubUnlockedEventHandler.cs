using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TennisClubEventHandlers;

public class TennisClubUnlockedEventHandler(ITennisClubReadModelRepository tennisClubReadModelRepository)
    : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }
        
        var tennisClub = await tennisClubReadModelRepository.GetTennisClubById(domainEnvelope.EntityId);
        
        if (tennisClub != null)
        {
            tennisClub.Unlock();
            await tennisClubReadModelRepository.Update();
        }
        else
        {
            // TODO: Add logging
            Console.WriteLine($"Could not unlock tennis club with id {domainEnvelope.EntityId}!");
        }
    }
    
    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TENNIS_CLUB_UNLOCKED);
    }
}