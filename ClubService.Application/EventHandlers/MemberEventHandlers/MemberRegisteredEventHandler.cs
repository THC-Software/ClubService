using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Member;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.EventHandlers.MemberEventHandlers;

public class MemberRegisteredEventHandler(
    IMemberReadModelRepository memberReadModelRepository,
    ITennisClubReadModelRepository tennisClubReadModelRepository,
    IReadStoreTransactionManager readStoreTransactionManager,
    ILoggerService<MemberRegisteredEventHandler> loggerService) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var memberRegisteredEvent = (MemberRegisteredEvent)domainEnvelope.EventData;
        var tennisClubReadModel =
            await tennisClubReadModelRepository.GetTennisClubById(memberRegisteredEvent.TennisClubId.Id);

        if (tennisClubReadModel == null)
        {
            loggerService.LogTennisClubNotFound(memberRegisteredEvent.TennisClubId.Id);
            return;
        }

        await readStoreTransactionManager.TransactionScope(async () =>
        {
            tennisClubReadModel.IncreaseMemberCount();
            await tennisClubReadModelRepository.Update();

            var memberReadModel = MemberReadModel.FromDomainEvent(memberRegisteredEvent);
            await memberReadModelRepository.Add(memberReadModel);
        });

        loggerService.LogMemberRegistered(memberRegisteredEvent.MemberId.Id);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.MEMBER_REGISTERED);
    }
}