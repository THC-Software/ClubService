using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;

namespace ClubService.Infrastructure.Repositories;

public class TournamentReadModelRepository(ReadStoreDbContext readStoreDbContext) : ITournamentReadModelRepository
{
    public async Task Add(TournamentReadModel tournamentReadModel)
    {
        await readStoreDbContext.Tournaments.AddAsync(tournamentReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }
}