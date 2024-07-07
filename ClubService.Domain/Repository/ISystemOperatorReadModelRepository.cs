using ClubService.Domain.ReadModel;

namespace ClubService.Domain.Repository;

public interface ISystemOperatorReadModelRepository
{
    Task Add(SystemOperatorReadModel systemOperatorReadModel);
    Task<SystemOperatorReadModel?> GetSystemOperatorByUsername(string username);
}