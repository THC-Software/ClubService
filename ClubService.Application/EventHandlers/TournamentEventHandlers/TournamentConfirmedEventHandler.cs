using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Tournament;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TournamentEventHandlers;

public class TournamentConfirmedEventHandler(
    ITennisClubReadModelRepository tennisClubReadModelRepository,
    ITournamentReadModelRepository tournamentReadModelRepository,
    IMemberReadModelRepository memberReadModelRepository,
    IEmailOutboxRepository emailOutboxRepository,
    ILoggerService<TournamentConfirmedEventHandler> loggerService)
    : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            loggerService.LogRejectEvent(domainEnvelope);
            return;
        }

        loggerService.LogHandleEvent(domainEnvelope);

        var tournamentConfirmedEvent = (TournamentConfirmedEvent)domainEnvelope.EventData;
        var tennisClubReadModel =
            await tennisClubReadModelRepository.GetTennisClubById(tournamentConfirmedEvent.ClubId);

        if (tennisClubReadModel == null)
        {
            loggerService.LogTennisClubNotFound(tournamentConfirmedEvent.ClubId);
            throw new TennisClubNotFoundException(domainEnvelope.EntityId);
        }

        var tournamentReadModel = TournamentReadModel.FromDomainEvent(tournamentConfirmedEvent);
        await tournamentReadModelRepository.Add(tournamentReadModel);

        var members = await memberReadModelRepository.GetMembersByTennisClubId(tennisClubReadModel.TennisClubId.Id);
        foreach (var member in members)
        {
            var emailMessage = new EmailMessage(Guid.NewGuid(), member.Email,
                tournamentConfirmedEvent.Name, tournamentConfirmedEvent.Description, DateTime.UtcNow);

            await emailOutboxRepository.Add(emailMessage);
        }

        loggerService.LogTournamentConfirmed(tournamentReadModel.TournamentId);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TOURNAMENT_CONFIRMED);
    }
}