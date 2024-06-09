using ClubService.Domain.Model.Entity;
using ClubService.Infrastructure.EntityConfigurations.Login;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace ClubService.Infrastructure.DbContexts;

public class LoginStoreDbContext(DbContextOptions<EventStoreDbContext> options, IHostEnvironment env) : DbContext(options)
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
    }
}