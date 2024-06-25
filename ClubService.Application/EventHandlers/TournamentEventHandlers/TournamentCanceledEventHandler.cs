using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.TournamentEventHandlers;

public class TournamentCanceledEventHandler(
    ITournamentReadModelRepository tournamentReadModelRepository,
    ITennisClubReadModelRepository tennisClubReadModelRepository,
    IMemberReadModelRepository memberReadModelRepository,
    IMailService mailService,
    ILoggerService<TournamentCanceledEventHandler> loggerService)
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

        var tournamentReadModel = await tournamentReadModelRepository.GetTournamentById(domainEnvelope.EntityId);

        if (tournamentReadModel == null)
        {
            loggerService.LogTournamentNotFound(domainEnvelope.EntityId);
            throw new TournamentNotFoundException(domainEnvelope.EntityId);
        }

        var tennisClubReadModel =
            await tennisClubReadModelRepository.GetTennisClubById(tournamentReadModel.TennisClubId);

        if (tennisClubReadModel == null)
        {
            loggerService.LogTennisClubNotFound(tournamentReadModel.TennisClubId);
            throw new TennisClubNotFoundException(domainEnvelope.EntityId);
        }

        await tournamentReadModelRepository.Delete(tournamentReadModel);

        var members = await memberReadModelRepository.GetMembersByTennisClubId(tennisClubReadModel.TennisClubId.Id);
        var mailSubject = $"Tournament {tournamentReadModel.Name} canceled";
        var mailBody = $"""
                        Unfortunately the tournament '{tournamentReadModel.Name}' that would have been taking 
                        place from the {tournamentReadModel.StartDate} to the {tournamentReadModel.EndDate} has 
                        been canceled.
                        """;

        foreach (var member in members)
        {
            await mailService.Send(member.Email, mailSubject, mailBody);
        }

        loggerService.LogTournamentCanceled(tournamentReadModel.TournamentId);
    }

    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.TOURNAMENT_CANCELED);
    }
}