using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure.Repositories;

public class LoginRepository(LoginStoreDbContext loginStoreDbContext) : ILoginRepository
{
    public async Task Add(UserPassword userPassword)
    {
        await loginStoreDbContext.UserPasswords.AddAsync(userPassword);
        await loginStoreDbContext.SaveChangesAsync();
    }

    public async Task ChangePassword()
    {
        await loginStoreDbContext.SaveChangesAsync();
    }

    public async Task<UserPassword?> GetById(Guid id)
    {
        return await loginStoreDbContext.UserPasswords
            .Where(up => up.UserId == new UserId(id))
            .SingleOrDefaultAsync();
    }
}