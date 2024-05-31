using ClubService.Domain.ReadModel;

namespace ClubService.Domain.Repository;

public interface ITennisClubReadModelRepository
{
    void Add(TennisClubReadModel tennisClubReadModel);
}