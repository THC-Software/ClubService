using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.MemberEventHandlers;

public class MemberFullNameChangedEventHandler(
    IMemberReadModelRepository memberReadModelRepository,
    ILoggerService<MemberFullNameChangedEventHandler> loggerService) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var memberFullNameChangedEvent = (MemberFullNameChangedEvent)domainEnvelope.EventData;
        var memberReadModel = await memberReadModelRepository.GetMemberById(domainEnvelope.EntityId);

        if (memberReadModel == null)
        {
            loggerService.LogMemberNotFound(domainEnvelope.EntityId);
            return;
        }

        memberReadModel.ChangeFullName(memberFullNameChangedEvent.Name);
        await memberReadModelRepository.Update();
        loggerService.LogMemberFullNameChanged(memberReadModel.MemberId.Id);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.MEMBER_FULL_NAME_CHANGED);
    }
}