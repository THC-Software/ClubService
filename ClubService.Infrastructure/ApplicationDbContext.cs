using ClubService.Domain.Event;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Enum;
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
            // Subscription Tiers
            new DomainEnvelope<IDomainEvent>(
                new Guid("8d4d3eff-b77b-4e21-963b-e211366bb94b"),
                new Guid("38888969-d579-46ec-9cd6-0208569a077e"),
                EventType.SUBSCRIPTION_TIER_CREATED,
                EntityType.SUBSCRIPTION_TIER,
                DateTime.UtcNow,
                new SubscriptionTierCreatedEvent(
                    new SubscriptionTierId(new Guid("38888969-d579-46ec-9cd6-0208569a077e")),
                    "Gorilla Subscription Tier",
                    200)
            ),
            new DomainEnvelope<IDomainEvent>(
                new Guid("e335d85a-f844-4c7e-b608-035ef00af733"),
                new Guid("d19073ba-f760-4a9a-abfa-f8215d96bec7"),
                EventType.SUBSCRIPTION_TIER_CREATED,
                EntityType.SUBSCRIPTION_TIER,
                DateTime.UtcNow,
                new SubscriptionTierCreatedEvent(
                    new SubscriptionTierId(new Guid("d19073ba-f760-4a9a-abfa-f8215d96bec7")),
                    "Bison Subscription Tier",
                    150)
            ),
            new DomainEnvelope<IDomainEvent>(
                new Guid("36db98d7-8fea-4715-923c-74192b147752"),
                new Guid("4c148d45-ebc8-4bbf-aa9a-d491eb185ad5"),
                EventType.SUBSCRIPTION_TIER_CREATED,
                EntityType.SUBSCRIPTION_TIER,
                DateTime.UtcNow,
                new SubscriptionTierCreatedEvent(
                    new SubscriptionTierId(new Guid("4c148d45-ebc8-4bbf-aa9a-d491eb185ad5")),
                    "Guinea Pig Subscription Tier",
                    150)
            ),
            new DomainEnvelope<IDomainEvent>(
                new Guid("3b591696-d9c9-4e30-a6a1-6a1439c5580b"),
                new Guid("2bebd11c-bf8e-4448-886f-0cb8608af7ca"),
                EventType.SUBSCRIPTION_TIER_CREATED,
                EntityType.SUBSCRIPTION_TIER,
                DateTime.UtcNow,
                new SubscriptionTierCreatedEvent(
                    new SubscriptionTierId(new Guid("2bebd11c-bf8e-4448-886f-0cb8608af7ca")),
                    "Woolf Subscription Tier",
                    100)
            ),
            // Tennis Clubs
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
                    new SubscriptionTierId(new Guid("d19073ba-f760-4a9a-abfa-f8215d96bec7")),
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
                    new SubscriptionTierId(new Guid("38888969-d579-46ec-9cd6-0208569a077e")),
                    [])
            ),
            new DomainEnvelope<IDomainEvent>(
                new Guid("b0457bfe-e8c5-4831-a10f-9e66490b4332"),
                new Guid("6a463e1a-6b0f-4825-83c3-911f12f80076"),
                EventType.TENNIS_CLUB_LOCKED,
                EntityType.TENNIS_CLUB,
                DateTime.UtcNow.AddHours(1),
                new TennisClubLockedEvent()
            ),
            // Members
            new DomainEnvelope<IDomainEvent>(
                new Guid("20a699d7-1bf8-4e0e-823c-82cafb246611"),
                new Guid("60831440-06d2-4017-9a7b-016e9cd0b2dc"),
                EventType.MEMBER_REGISTERED,
                EntityType.MEMBER,
                DateTime.UtcNow,
                new MemberRegisteredEvent(
                    new MemberId(new Guid("60831440-06d2-4017-9a7b-016e9cd0b2dc")),
                    new FullName("Adrian", "Spiegel"),
                    "adrianSpiegel@fhv.gorillaKaefig",
                    new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3")),
                    MemberStatus.NONE
                )
            )
        );
    }
}