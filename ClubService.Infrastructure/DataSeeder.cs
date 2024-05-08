using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Infrastructure;

public class DataSeeder(ApplicationDbContext applicationDbContext)
{
    public async Task SeedTestData()
    {
        applicationDbContext.DomainEvents.Add(new DomainEnvelope<IDomainEvent>(
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
            )
        );
        
        await applicationDbContext.SaveChangesAsync();
    }
}