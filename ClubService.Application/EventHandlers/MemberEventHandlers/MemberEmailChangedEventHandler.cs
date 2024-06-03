using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.MemberEventHandlers;

public class MemberEmailChangedEventHandler(IMemberReadModelRepository memberReadModelRepository) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }
        
        var memberEmailChangedEvent = (MemberEmailChangedEvent)domainEnvelope.EventData;
        var memberReadModel = await memberReadModelRepository.GetMemberById(domainEnvelope.EntityId);
        
        if (memberReadModel == null)
        {
            // TODO: Add logging
            Console.WriteLine($"Member with id {domainEnvelope.EntityId} not found!");
            return;
        }
        
        memberReadModel.ChangeEmail(memberEmailChangedEvent.Email);
        await memberReadModelRepository.Update();
    }
    
    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.MEMBER_EMAIL_CHANGED);
    }
}