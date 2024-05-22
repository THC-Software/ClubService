using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.IntegrationTests.TestSetup;

namespace ClubService.IntegrationTests.Repository;

[TestFixture]
public class EventRepositoryTests : TestBase
{
    [Test]
    public async Task GivenDomainEvent_WhenSave_ThenEventIsSavedInDatabase()
    {
        // Given
        var tennisClubRegisteredEventExpected =
            new TennisClubRegisteredEvent(new TennisClubId(Guid.NewGuid()), "Test Tennis Club",
                new SubscriptionTierId(Guid.NewGuid()), TennisClubStatus.NONE);
        var domainEnvelopeExpected =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(),
                tennisClubRegisteredEventExpected.TennisClubId.Id,
                EventType.TENNIS_CLUB_REGISTERED, EntityType.TENNIS_CLUB, DateTime.UtcNow,
                tennisClubRegisteredEventExpected);
        
        // When
        await EventRepository.Save(domainEnvelopeExpected);
        
        // Then
        var savedEvents =
            EventRepository.GetEventsForEntity<ITennisClubDomainEvent>(domainEnvelopeExpected.EntityId);
        Assert.That(savedEvents, Has.Count.EqualTo(1));
        
        var domainEnvelopeActual = savedEvents[0];
        Assert.Multiple(() =>
        {
            Assert.That(domainEnvelopeActual.EventId, Is.EqualTo(domainEnvelopeExpected.EventId));
            Assert.That(domainEnvelopeActual.EntityId, Is.EqualTo(domainEnvelopeExpected.EntityId));
            Assert.That(domainEnvelopeActual.EventType, Is.EqualTo(domainEnvelopeExpected.EventType));
            Assert.That(domainEnvelopeActual.EntityType, Is.EqualTo(domainEnvelopeExpected.EntityType));
            Assert.That(domainEnvelopeActual.Timestamp,
                Is.EqualTo(domainEnvelopeExpected.Timestamp).Within(TimeSpan.FromSeconds(1)));
            Assert.That(domainEnvelopeActual.EventData.GetType(),
                Is.EqualTo(domainEnvelopeExpected.EventData.GetType()));
        });
        
        var tennisClubRegisteredEventActual = (TennisClubRegisteredEvent)domainEnvelopeActual.EventData;
        Assert.Multiple(() =>
        {
            Assert.That(tennisClubRegisteredEventActual.TennisClubId,
                Is.EqualTo(tennisClubRegisteredEventExpected.TennisClubId));
            Assert.That(tennisClubRegisteredEventActual.Name, Is.EqualTo(tennisClubRegisteredEventExpected.Name));
            Assert.That(tennisClubRegisteredEventActual.Status,
                Is.EqualTo(tennisClubRegisteredEventExpected.Status));
            Assert.That(tennisClubRegisteredEventActual.SubscriptionTierId,
                Is.EqualTo(tennisClubRegisteredEventExpected.SubscriptionTierId));
        });
    }
}