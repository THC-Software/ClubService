using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Member;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.MemberEventHandlers;

public class MemberRegisteredEventHandler(IMemberReadModelRepository memberReadModelRepository) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }
        
        var memberRegisteredEvent = (MemberRegisteredEvent)domainEnvelope.EventData;
        var memberReadModel = MemberReadModel.FromDomainEvent(memberRegisteredEvent);
        
        await memberReadModelRepository.Add(memberReadModel);
    }
    
    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.MEMBER_REGISTERED);
    }
}