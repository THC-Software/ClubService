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
        var startDate = tournamentConfirmedEvent.Tournament.Days.OrderBy(d => d.Day).First();
        var endDate = tournamentConfirmedEvent.Tournament.Days.OrderBy(d => d.Day).Last();

        return new TournamentReadModel(
            tournamentConfirmedEvent.Tournament.Id,
            tournamentConfirmedEvent.Tournament.ClubId,
            tournamentConfirmedEvent.Tournament.Name,
            startDate.Day,
            endDate.Day
        );
    }
}