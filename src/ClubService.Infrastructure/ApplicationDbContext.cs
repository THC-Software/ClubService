using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.ValueObject;
using ClubService.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace ClubService.Infrastructure;

public class ApplicationDbContext(DbContextOptions options, IHostEnvironment env) : DbContext(options)
{
    public DbSet<DomainEnvelope<IDomainEvent>> DomainEvents { get; init; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseCamelCaseNamingConvention();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new DomainEnvelopeConfiguration());
        
        if (!env.IsDevelopment() && !env.IsEnvironment("Test"))
        {
            return;
        }
        
        modelBuilder.Entity<DomainEnvelope<IDomainEvent>>().HasData(
            new DomainEnvelope<IDomainEvent>(
                new Guid("049af565-a42d-4f02-b213-793459e4873f"),
                new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3"),
                EventType.TENNIS_CLUB_REGISTERED,
                EntityType.TENNIS_CLUB,
                DateTime.UtcNow,
                new TennisClubRegisteredEvent(
                    new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3")),
                    "Tennis CLub 1",
                    false,
                    new SubscriptionTierId(new Guid("20698bde-5b82-4129-a72f-145ea96d8be7")),
                    [])
            ),
            new DomainEnvelope<IDomainEvent>(
                new Guid("e3d63ccb-7f1d-43c1-b44e-2221dce70998"),
                new Guid("6a463e1a-6b0f-4825-83c3-911f12f80076"),
                EventType.TENNIS_CLUB_REGISTERED,
                EntityType.TENNIS_CLUB,
                DateTime.UtcNow,
                new TennisClubRegisteredEvent(
                    new TennisClubId(new Guid("6a463e1a-6b0f-4825-83c3-911f12f80076")),
                    "Tennis CLub 2",
                    false,
                    new SubscriptionTierId(new Guid("20698bde-5b82-4129-a72f-145ea96d8be7")),
                    [])
            ),
            new DomainEnvelope<IDomainEvent>(
                new Guid("b0457bfe-e8c5-4831-a10f-9e66490b4332"),
                new Guid("6a463e1a-6b0f-4825-83c3-911f12f80076"),
                EventType.TENNIS_CLUB_LOCKED,
                EntityType.TENNIS_CLUB,
                DateTime.UtcNow.AddHours(1),
                new TennisClubLockedEvent()
            )
        );
    }
}