using ClubService.Domain.ReadModel;

namespace ClubService.Domain.Repository;

public interface IAdminReadModelRepository
{
    Task Add(AdminReadModel adminReadModel);
    Task Delete(AdminReadModel adminReadModel);
    Task Update();
    Task<AdminReadModel?> GetAdminById(Guid id);
    Task<List<AdminReadModel>> GetAdminsByTennisClubId(Guid tennisClubId);
    Task<AdminReadModel?> GetAdminByTennisClubIdAndUsername(Guid tennisClubId, string username);
}