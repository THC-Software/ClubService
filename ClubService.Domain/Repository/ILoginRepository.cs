using ClubService.Domain.Model.Entity;

namespace ClubService.Domain.Repository;

public interface ILoginRepository
{
    Task Add(UserPassword userPassword);
    Task<UserPassword?> GetById(Guid id);
}