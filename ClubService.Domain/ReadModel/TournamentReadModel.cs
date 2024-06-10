using ClubService.Domain.Event.Tournament;

namespace ClubService.Domain.ReadModel;

public class TournamentReadModel
{
    private TournamentReadModel(Guid tournamentId, Guid tennisClubId, string name, DateOnly startDate, DateOnly endDate)
    {
        TournamentId = tournamentId;
        TennisClubId = tennisClubId;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }

    public Guid TournamentId { get; }
    public Guid TennisClubId { get; }
    public string Name { get; }
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; }

    public static TournamentReadModel FromDomainEvent(TournamentConfirmedEvent tournamentConfirmedEvent)
    {
        var startDate = tournamentConfirmedEvent.Days.First();
        var endDate = tournamentConfirmedEvent.Days.Last();

        return new TournamentReadModel(
            tournamentConfirmedEvent.Id,
            tournamentConfirmedEvent.ClubId,
            tournamentConfirmedEvent.Name,
            startDate.Day,
            endDate.Day
        );
    }
}