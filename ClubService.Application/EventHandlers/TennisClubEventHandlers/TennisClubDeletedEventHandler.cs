using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TennisClubEventHandlers;

public class TennisClubDeletedEventHandler(ITennisClubReadModelRepository tennisClubReadModelRepository) : IEventHandler
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
            await tennisClubReadModelRepository.Delete(tennisClub);
        }
        else
        {
            // TODO: Add logging
            Console.WriteLine($"Tennis club with id {domainEnvelope.EntityId} does not exist!");
        }
    }
    
    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TENNIS_CLUB_DELETED);
    }
}