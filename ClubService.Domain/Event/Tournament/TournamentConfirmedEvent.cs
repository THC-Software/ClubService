namespace ClubService.Domain.Event.Tournament;

public class TournamentConfirmedEvent(Model.Entity.Tournament tournament) : ITournamentDomainEvent
{
    public Model.Entity.Tournament Tournament { get; } = tournament;
}