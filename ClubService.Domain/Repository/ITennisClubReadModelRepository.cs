using ClubService.Domain.ReadModel;

namespace ClubService.Domain.Repository;

public interface ITennisClubReadModelRepository
{
    Task Add(TennisClubReadModel tennisClubReadModel);
    Task Update();
    Task<TennisClubReadModel?> GetTennisClubById(Guid id);
}