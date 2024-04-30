using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.UnitTests;

[TestFixture]
public class TennisClubTests
{
    [Test]
    public void
        GivenValidInputs_WhenProcessTennisClubRegisterCommand_ThenReturnsDomainEnvelopeWithTennisClubRegisteredEvent()
    {
        // Given
        var tennisClubIdExpected = new TennisClubId(Guid.NewGuid());
        var nameExpected = "Test Tennis Club";
        var isLockedExpected = false;
        var subscriptionTierIdExpected = new SubscriptionTierId(Guid.NewGuid());
        var memberIdsCountExpected = 0;
        var tennisClub = TennisClub.Create(tennisClubIdExpected);

        // When
        var domainEnvelopes =
            tennisClub.ProcessTennisClubRegisterCommand(nameExpected, subscriptionTierIdExpected.Id.ToString());

        // Then
        Assert.That(domainEnvelopes, Is.Not.Null);
        Assert.That(domainEnvelopes, Has.Count.EqualTo(1));

        var domainEnvelope = domainEnvelopes[0];
        Assert.That(domainEnvelope.EntityId, Is.EqualTo(tennisClubIdExpected.Id));
        Assert.That(domainEnvelope.EventType, Is.EqualTo(EventType.TENNIS_CLUB_REGISTERED));
        Assert.That(domainEnvelope.EntityType, Is.EqualTo(EntityType.TENNIS_CLUB));

        var tennisClubRegisteredEvent = domainEnvelope.EventData as TennisClubRegisteredEvent;
        Assert.That(tennisClubRegisteredEvent, Is.Not.Null);
        Assert.That(tennisClubRegisteredEvent.TennisClubId, Is.EqualTo(tennisClubIdExpected));
        Assert.That(tennisClubRegisteredEvent.Name, Is.EqualTo(nameExpected));
        Assert.That(tennisClubRegisteredEvent.IsLocked, Is.EqualTo(isLockedExpected));
        Assert.That(tennisClubRegisteredEvent.SubscriptionTierId, Is.EqualTo(subscriptionTierIdExpected));
        Assert.That(tennisClubRegisteredEvent.MemberIds, Has.Count.EqualTo(memberIdsCountExpected));
    }

    [Test]
    public void GivenTennisClubRegisteredEvent_WhenApply_ThenTennisClubIsInExpectedState()
    {
        // Given
        var tennisClubIdExpected = new TennisClubId(Guid.NewGuid());
        var nameExpected = "Test Tennis Club";
        var isLockedExpected = false;
        var subscriptionTierIdExpected = new SubscriptionTierId(Guid.NewGuid());
        List<MemberId> memberIdsExpected = [];
        var tennisClub = TennisClub.Create(tennisClubIdExpected);

        var tennisClubRegisteredEvent =
            new TennisClubRegisteredEvent(tennisClubIdExpected, nameExpected, isLockedExpected,
                subscriptionTierIdExpected, memberIdsExpected);

        var domainEnvelope =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubIdExpected.Id,
                EventType.TENNIS_CLUB_REGISTERED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubRegisteredEvent);

        // When
        tennisClub.Apply(domainEnvelope);

        // Then
        Assert.That(tennisClub.TennisClubId, Is.EqualTo(tennisClubIdExpected));
        Assert.That(tennisClub.Name, Is.EqualTo(nameExpected));
        Assert.That(tennisClub.IsLocked, Is.EqualTo(isLockedExpected));
        Assert.That(tennisClub.SubscriptionTierId, Is.EqualTo(subscriptionTierIdExpected));
        Assert.That(tennisClub.MemberIds, Is.EqualTo(memberIdsExpected));
    }
}