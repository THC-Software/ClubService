using ClubService.Domain.Model.Entity;
using ClubService.Infrastructure.EntityConfigurations.Login;
using ClubService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace ClubService.Infrastructure.DbContexts;

public class LoginStoreDbContext(DbContextOptions<LoginStoreDbContext> options, IHostEnvironment env)
    : DbContext(options)
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

        modelBuilder.Entity<UserPassword>().HasData(
            // System Operator
            UserPassword.Create(
                new Guid("1588ec27-c932-4dee-a341-d18c8108a711"),
                "systemoperator",
                passwordHasherService
            )
        );

        if (env.IsProduction())
        {
            return;
        }

        modelBuilder.Entity<UserPassword>().HasData(
            // Admins
            UserPassword.Create(
                new Guid("1dd88382-f781-4bf8-94e3-05e99d1434fe"),
                "test",
                passwordHasherService
            ),
            UserPassword.Create(
                new Guid("4a2eb3dc-7f1e-4dac-851a-667594ca31ff"),
                "test",
                passwordHasherService
            ),
            UserPassword.Create(
                new Guid("5d2f1aec-1cc6-440a-b04f-ba8b3085a35a"),
                "test",
                passwordHasherService
            ),

            // Members
            UserPassword.Create(
                new Guid("60831440-06d2-4017-9a7b-016e9cd0b2dc"),
                "test",
                passwordHasherService
            ),
            UserPassword.Create(
                new Guid("51ae7aca-2bb8-421a-a923-2ba2eb94bb3a"),
                "test",
                passwordHasherService
            ),
            UserPassword.Create(
                new Guid("e8a2cd4c-69ad-4cf2-bca6-a60d88be6649"),
                "test",
                passwordHasherService
            )
        );
    }
}