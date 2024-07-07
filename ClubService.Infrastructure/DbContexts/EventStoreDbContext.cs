using ClubService.Domain.Event;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Event.SystemOperator;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure.DbContexts;

public class EventStoreDbContext(DbContextOptions<EventStoreDbContext> options)
    : DbContext(options)
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

        modelBuilder.Entity<DomainEnvelope<IDomainEvent>>().HasData(
            new DomainEnvelope<IDomainEvent>(
                new Guid("ba4eeaa4-9707-4da0-8ee2-3684b4f7804f"),
                new Guid("1588ec27-c932-4dee-a341-d18c8108a711"),
                EventType.SYSTEM_OPERATOR_REGISTERED,
                EntityType.SYSTEM_OPERATOR,
                DateTime.UtcNow,
                new SystemOperatorRegisteredEvent(
                    new SystemOperatorId(new Guid("1588ec27-c932-4dee-a341-d18c8108a711")), "systemoperator")
            )
        );
    }

    public async Task SeedTestData()
    {
        // We need to seed the test data like this so that the events are inserted in the right order into the db
        // With HasData the order is not the same like in the code and then debezium fetches the events in the wrong order
        var domainEvents = new List<DomainEnvelope<IDomainEvent>>
        {
            // Subscription Tiers
            new(
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
            new(
                new Guid("e335d85a-f844-4c7e-b608-035ef00af733"),
                new Guid("d19073ba-f760-4a9a-abfa-f8215d96bec7"),
                EventType.SUBSCRIPTION_TIER_CREATED,
                EntityType.SUBSCRIPTION_TIER,
                DateTime.UtcNow,
                new SubscriptionTierCreatedEvent(
                    new SubscriptionTierId(new Guid("d19073ba-f760-4a9a-abfa-f8215d96bec7")),
                    "Bison Subscription Tier",
                    250)
            ),
            new(
                new Guid("36db98d7-8fea-4715-923c-74192b147752"),
                new Guid("4c148d45-ebc8-4bbf-aa9a-d491eb185ad5"),
                EventType.SUBSCRIPTION_TIER_CREATED,
                EntityType.SUBSCRIPTION_TIER,
                DateTime.UtcNow,
                new SubscriptionTierCreatedEvent(
                    new SubscriptionTierId(new Guid("4c148d45-ebc8-4bbf-aa9a-d491eb185ad5")),
                    "Guinea Pig Subscription Tier",
                    100)
            ),
            new(
                new Guid("3b591696-d9c9-4e30-a6a1-6a1439c5580b"),
                new Guid("2bebd11c-bf8e-4448-886f-0cb8608af7ca"),
                EventType.SUBSCRIPTION_TIER_CREATED,
                EntityType.SUBSCRIPTION_TIER,
                DateTime.UtcNow,
                new SubscriptionTierCreatedEvent(
                    new SubscriptionTierId(new Guid("2bebd11c-bf8e-4448-886f-0cb8608af7ca")),
                    "Woolf Subscription Tier",
                    150)
            ),

            // Tennis Clubs
            new(
                new Guid("049af565-a42d-4f02-b213-793459e4873f"),
                new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3"),
                EventType.TENNIS_CLUB_REGISTERED,
                EntityType.TENNIS_CLUB,
                DateTime.UtcNow,
                new TennisClubRegisteredEvent(
                    new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3")),
                    "Tennis CLub 1",
                    new SubscriptionTierId(new Guid("d19073ba-f760-4a9a-abfa-f8215d96bec7")),
                    TennisClubStatus.ACTIVE)
            ),
            new(
                new Guid("e3d63ccb-7f1d-43c1-b44e-2221dce70998"),
                new Guid("6a463e1a-6b0f-4825-83c3-911f12f80076"),
                EventType.TENNIS_CLUB_REGISTERED,
                EntityType.TENNIS_CLUB,
                DateTime.UtcNow,
                new TennisClubRegisteredEvent(
                    new TennisClubId(new Guid("6a463e1a-6b0f-4825-83c3-911f12f80076")),
                    "Tennis CLub 2",
                    new SubscriptionTierId(new Guid("38888969-d579-46ec-9cd6-0208569a077e")),
                    TennisClubStatus.ACTIVE)
            ),
            new(
                new Guid("b0457bfe-e8c5-4831-a10f-9e66490b4332"),
                new Guid("6a463e1a-6b0f-4825-83c3-911f12f80076"),
                EventType.TENNIS_CLUB_LOCKED,
                EntityType.TENNIS_CLUB,
                DateTime.UtcNow.AddMilliseconds(10.0),
                new TennisClubLockedEvent()
            ),

            // Members
            new(
                new Guid("20a699d7-1bf8-4e0e-823c-82cafb246611"),
                new Guid("60831440-06d2-4017-9a7b-016e9cd0b2dc"),
                EventType.MEMBER_REGISTERED,
                EntityType.MEMBER,
                DateTime.UtcNow,
                new MemberRegisteredEvent(
                    new MemberId(new Guid("60831440-06d2-4017-9a7b-016e9cd0b2dc")),
                    new FullName("Adrian", "Essig"),
                    "adrian.essig@fhv.gorillaKaefig",
                    new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3")),
                    MemberStatus.ACTIVE
                )
            ),
            new(
                new Guid("16f3386e-4b6f-4e12-9599-fc4e0d5fa551"),
                new Guid("51ae7aca-2bb8-421a-a923-2ba2eb94bb3a"),
                EventType.MEMBER_REGISTERED,
                EntityType.MEMBER,
                DateTime.UtcNow,
                new MemberRegisteredEvent(
                    new MemberId(new Guid("51ae7aca-2bb8-421a-a923-2ba2eb94bb3a")),
                    new FullName("John", "Doe"),
                    "john.doe@fhv.gorillaKaefig",
                    new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3")),
                    MemberStatus.ACTIVE
                )
            ),
            new(
                new Guid("83396767-c873-4e11-95b9-f6d6abc4abd1"),
                new Guid("51ae7aca-2bb8-421a-a923-2ba2eb94bb3a"),
                EventType.MEMBER_LOCKED,
                EntityType.MEMBER,
                DateTime.UtcNow.AddMilliseconds(10.0),
                new MemberLockedEvent()
            ),
            new(
                new Guid("71106aed-ad91-4a3a-98b2-1d297c3ca335"),
                new Guid("e8a2cd4c-69ad-4cf2-bca6-a60d88be6649"),
                EventType.MEMBER_REGISTERED,
                EntityType.MEMBER,
                DateTime.UtcNow,
                new MemberRegisteredEvent(
                    new MemberId(new Guid("e8a2cd4c-69ad-4cf2-bca6-a60d88be6649")),
                    new FullName("John", "Doe"),
                    "john.moe@fhv.gorillaKaefig",
                    new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3")),
                    MemberStatus.ACTIVE
                )
            ),
            new(
                new Guid("fd3e1a64-7dfc-475e-a62b-cd6398f5ad74"),
                new Guid("e8a2cd4c-69ad-4cf2-bca6-a60d88be6649"),
                EventType.MEMBER_FULL_NAME_CHANGED,
                EntityType.MEMBER,
                DateTime.UtcNow.AddMilliseconds(10.0),
                new MemberFullNameChangedEvent(
                    new FullName("Michael", "Doe")
                )
            ),
            new(
                new Guid("890a8c5b-11b6-401d-824f-f000d302031d"),
                new Guid("e8a2cd4c-69ad-4cf2-bca6-a60d88be6649"),
                EventType.MEMBER_EMAIL_CHANGED,
                EntityType.MEMBER,
                DateTime.UtcNow.AddMilliseconds(100.0),
                new MemberEmailChangedEvent("jane.doe@fhv.gorillaKaefig")
            ),
            new(
                new Guid("7e13eab3-169e-40ec-87fe-3facc64c81bb"),
                new Guid("e8a2cd4c-69ad-4cf2-bca6-a60d88be6649"),
                EventType.MEMBER_DELETED,
                EntityType.MEMBER,
                DateTime.UtcNow.AddMilliseconds(1000.0),
                new MemberDeletedEvent()
            ),

            // Admins
            new(
                new Guid("46162bac-2fcd-4511-9713-2777b1bad883"),
                new Guid("1dd88382-f781-4bf8-94e3-05e99d1434fe"),
                EventType.ADMIN_REGISTERED,
                EntityType.ADMIN,
                DateTime.UtcNow,
                new AdminRegisteredEvent(
                    new AdminId(new Guid("1dd88382-f781-4bf8-94e3-05e99d1434fe")),
                    "adrian",
                    new FullName("Adrian", "Essig"),
                    new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3")),
                    AdminStatus.ACTIVE
                )
            ),
            new(
                new Guid("94b8e057-02a1-4d34-8bb1-b136c8ec3029"),
                new Guid("4a2eb3dc-7f1e-4dac-851a-667594ca31ff"),
                EventType.ADMIN_REGISTERED,
                EntityType.ADMIN,
                DateTime.UtcNow,
                new AdminRegisteredEvent(
                    new AdminId(new Guid("4a2eb3dc-7f1e-4dac-851a-667594ca31ff")),
                    "quarco",
                    new FullName("Marco", "Pressure"),
                    new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3")),
                    AdminStatus.ACTIVE
                )
            ),
            new(
                new Guid("4d3d8403-f0a3-4d8a-a610-af4dfeb77124"),
                new Guid("5d2f1aec-1cc6-440a-b04f-ba8b3085a35a"),
                EventType.ADMIN_REGISTERED,
                EntityType.ADMIN,
                DateTime.UtcNow,
                new AdminRegisteredEvent(
                    new AdminId(new Guid("5d2f1aec-1cc6-440a-b04f-ba8b3085a35a")),
                    "michael_essig",
                    new FullName("Michael", "Mirror"),
                    new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3")),
                    AdminStatus.ACTIVE
                )
            ),
            new(
                new Guid("4e252abf-08e9-4e6d-a674-f99d4ee740d5"),
                new Guid("5d2f1aec-1cc6-440a-b04f-ba8b3085a35a"),
                EventType.ADMIN_FULL_NAME_CHANGED,
                EntityType.ADMIN,
                DateTime.UtcNow.AddMilliseconds(10.0),
                new AdminFullNameChangedEvent(
                    new FullName("Michael", "Spiegel")
                )
            ),
            new(
                new Guid("b9832de8-542e-4a3c-b745-b39f8ffc3223"),
                new Guid("5d2f1aec-1cc6-440a-b04f-ba8b3085a35a"),
                EventType.ADMIN_DELETED,
                EntityType.ADMIN,
                DateTime.UtcNow.AddMilliseconds(1000.0),
                new AdminDeletedEvent()
            )
        };

        foreach (var domainEvent in domainEvents)
        {
            await DomainEvents.AddAsync(domainEvent);
            await SaveChangesAsync();
        }
    }
}