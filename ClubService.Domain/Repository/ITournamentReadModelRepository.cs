using ClubService.Domain.ReadModel;

namespace ClubService.Domain.Repository;

public interface ITournamentReadModelRepository
{
    Task Add(TournamentReadModel tournamentReadModel);
}