﻿using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.MemberEventHandlers;

public class MemberUnlockedEventHandler(
    IMemberReadModelRepository memberReadModelRepository,
    ILoggerService<MemberUnlockedEventHandler> loggerService) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var memberReadModel = await memberReadModelRepository.GetMemberById(domainEnvelope.EntityId);

        if (memberReadModel == null)
        {
            loggerService.LogMemberNotFound(domainEnvelope.EntityId);
            return;
        }

        memberReadModel.Unlock();
        await memberReadModelRepository.Update();
        loggerService.LogMemberUnlocked(memberReadModel.MemberId.Id);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.MEMBER_UNLOCKED);
    }
}