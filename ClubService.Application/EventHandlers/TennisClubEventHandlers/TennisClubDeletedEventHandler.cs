using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.EventHandlers.TennisClubEventHandlers;

public class TennisClubDeletedEventHandler(
    ITennisClubReadModelRepository tennisClubReadModelRepository,
    IAdminReadModelRepository adminReadModelRepository,
    IMemberReadModelRepository memberReadModelRepository,
    ILoggerService<TennisClubDeletedEventHandler> loggerService) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            loggerService.LogRejectEvent(domainEnvelope);
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var tennisClubReadModel = await tennisClubReadModelRepository.GetTennisClubById(domainEnvelope.EntityId);

        if (tennisClubReadModel == null)
        {
            loggerService.LogTennisClubNotFound(domainEnvelope.EntityId);
            return;
        }

        await memberReadModelRepository.DeleteMembersByTennisClubId(tennisClubReadModel.TennisClubId.Id);
        await adminReadModelRepository.DeleteAdminsByTennisClubId(tennisClubReadModel.TennisClubId.Id);
        await tennisClubReadModelRepository.Delete(tennisClubReadModel);

        loggerService.LogTennisClubDeleted(tennisClubReadModel.TennisClubId.Id);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TENNIS_CLUB_DELETED);
    }
}