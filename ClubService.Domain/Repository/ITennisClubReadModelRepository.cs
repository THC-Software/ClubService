using ClubService.Domain.ReadModel;

namespace ClubService.Domain.Repository;

public interface ITennisClubReadModelRepository
{
    Task Add(TennisClubReadModel tennisClubReadModel);
    Task Update();
    Task<TennisClubReadModel?> GetTennisClubById(Guid id);
    Task Delete(TennisClubReadModel tennisClub);
    Task<List<TennisClubReadModel>> GetAllTennisClubs(int pageSize = 0, int pageNumber = 1);
}