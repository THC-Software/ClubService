using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.MemberEventHandlers;

public class MemberEmailChangedEventHandler(
    IMemberReadModelRepository memberReadModelRepository,
    ILoggerService<MemberEmailChangedEventHandler> loggerService) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var memberEmailChangedEvent = (MemberEmailChangedEvent)domainEnvelope.EventData;
        var memberReadModel = await memberReadModelRepository.GetMemberById(domainEnvelope.EntityId);

        if (memberReadModel == null)
        {
            loggerService.LogMemberNotFound(domainEnvelope.EntityId);
            return;
        }

        memberReadModel.ChangeEmail(memberEmailChangedEvent.Email);
        await memberReadModelRepository.Update();
        loggerService.LogMemberEmailChanged(memberReadModel.MemberId.Id);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.MEMBER_EMAIL_CHANGED);
    }
}