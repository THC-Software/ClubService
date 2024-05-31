using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TennisClubEventHandlers;

public class TennisClubRegisteredEventHandler(ITennisClubReadModelRepository tennisClubReadModelRepository)
    : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }
        
        var tennisClub = TennisClubReadModel.FromDomainEvent((TennisClubRegisteredEvent)domainEnvelope.EventData);
        await tennisClubReadModelRepository.Add(tennisClub);
    }
    
    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TENNIS_CLUB_REGISTERED);
    }
}