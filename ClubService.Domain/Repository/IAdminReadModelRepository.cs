using ClubService.Domain.ReadModel;

namespace ClubService.Domain.Repository;

public interface IAdminReadModelRepository
{
    Task Add(AdminReadModel adminReadModel);
    Task Delete(AdminReadModel adminReadModel);
}