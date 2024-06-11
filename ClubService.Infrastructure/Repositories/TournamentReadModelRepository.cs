using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure.Repositories;

public class TournamentReadModelRepository(ReadStoreDbContext readStoreDbContext) : ITournamentReadModelRepository
{
    public async Task Add(TournamentReadModel tournamentReadModel)
    {
        await readStoreDbContext.Tournaments.AddAsync(tournamentReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }

    public async Task<TournamentReadModel?> GetTournamentById(Guid id)
    {
        return await readStoreDbContext.Tournaments
            .Where(tournament => tournament.TournamentId == id)
            .SingleOrDefaultAsync();
    }

    public async Task Delete(TournamentReadModel tournamentReadModel)
    {
        readStoreDbContext.Tournaments.Remove(tournamentReadModel);
        await readStoreDbContext.SaveChangesAsync();
    }
}