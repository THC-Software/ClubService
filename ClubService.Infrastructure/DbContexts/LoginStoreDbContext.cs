using ClubService.Domain.Model.Entity;
using ClubService.Infrastructure.EntityConfigurations.Login;
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

    // TODO: Seed login store with initial default system operator
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserPasswordConfiguration());
    }
}