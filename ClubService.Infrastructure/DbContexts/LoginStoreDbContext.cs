using ClubService.Domain.Model.Entity;
using ClubService.Infrastructure.EntityConfigurations.Login;
using ClubService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure.DbContexts;

public class LoginStoreDbContext(DbContextOptions<LoginStoreDbContext> options) : DbContext(options)
{
    public DbSet<UserPassword> UserPasswords { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseCamelCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserPasswordConfiguration());

        var passwordHasherService = new PasswordHasherService();
        var initialSystemOperator = UserPassword.Create(
            new Guid("1588ec27-c932-4dee-a341-d18c8108a711"), "systemoperator", passwordHasherService);
        modelBuilder.Entity<UserPassword>().HasData(initialSystemOperator);
    }
}