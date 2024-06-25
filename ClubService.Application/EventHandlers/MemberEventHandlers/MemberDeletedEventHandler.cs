using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.MemberEventHandlers;

public class MemberDeletedEventHandler(
    IMemberReadModelRepository memberReadModelRepository,
    ITennisClubReadModelRepository tennisClubReadModelRepository,
    ILoggerService<MemberDeletedEventHandler> loggerService) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            loggerService.LogRejectEvent(domainEnvelope);
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var memberReadModel = await memberReadModelRepository.GetMemberById(domainEnvelope.EntityId);

        if (memberReadModel == null)
        {
            loggerService.LogMemberNotFound(domainEnvelope.EntityId);
            throw new MemberNotFoundException(domainEnvelope.EntityId);
        }

        var tennisClubReadModel =
            await tennisClubReadModelRepository.GetTennisClubById(memberReadModel.TennisClubId.Id);

        if (tennisClubReadModel == null)
        {
            loggerService.LogTennisClubNotFound(memberReadModel.TennisClubId.Id);
            throw new TennisClubNotFoundException(memberReadModel.TennisClubId.Id);
        }

        tennisClubReadModel.DecreaseMemberCount();
        await tennisClubReadModelRepository.Update();
        await memberReadModelRepository.Delete(memberReadModel);

        loggerService.LogMemberDeleted(memberReadModel.MemberId.Id);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.MEMBER_DELETED);
    }
}