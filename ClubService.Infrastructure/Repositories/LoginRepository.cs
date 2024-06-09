using ClubService.Domain.Model.Entity;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;

namespace ClubService.Infrastructure.Repositories;

public class LoginRepository(LoginStoreDbContext loginStoreDbContext) : ILoginRepository
{
    public async Task Add(UserPassword userPassword)
    {
        await loginStoreDbContext.UserPasswords.AddAsync(userPassword);
        await loginStoreDbContext.SaveChangesAsync();
    }
}