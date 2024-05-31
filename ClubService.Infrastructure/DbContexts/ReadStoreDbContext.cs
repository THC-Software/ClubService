using ClubService.Domain.ReadModel;
using ClubService.Infrastructure.EntityConfigurations.ReadModel;
using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure.DbContexts;

public class ReadStoreDbContext(DbContextOptions<ReadStoreDbContext> options) : DbContext(options)
{
    public DbSet<SubscriptionTierReadModel> SubscriptionTiers { get; init; }
    public DbSet<TennisClubReadModel> TennisClubs { get; init; }
    public DbSet<MemberReadModel> Members { get; init; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseCamelCaseNamingConvention();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new SubscriptionTierReadModelConfiguration());
        modelBuilder.ApplyConfiguration(new TennisClubReadModelConfiguration());
        modelBuilder.ApplyConfiguration(new MemberReadModelConfiguration());
    }
}