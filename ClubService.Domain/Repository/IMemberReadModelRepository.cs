using ClubService.Domain.ReadModel;

namespace ClubService.Domain.Repository;

public interface IMemberReadModelRepository
{
    Task Add(MemberReadModel memberReadModel);
    Task Update();
}