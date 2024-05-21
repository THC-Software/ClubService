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
        GivenValidInputs_WhenProcessTennisClubRegisterCommand_ThenDomainEnvelopeWithTennisClubRegisteredEventIsReturned()
    {
        // Given
        var eventTypeExpected = EventType.TENNIS_CLUB_REGISTERED;
        var entityTypeExpected = EntityType.TENNIS_CLUB;
        var nameExpected = "Test Tennis Club";
        var isLockedExpected = false;
        var subscriptionTierIdExpected = new SubscriptionTierId(Guid.NewGuid());
        var memberIdsCountExpected = 0;
        var tennisClub = new TennisClub();
        
        // When
        var domainEnvelopes =
            tennisClub.ProcessTennisClubRegisterCommand(nameExpected, subscriptionTierIdExpected.Id.ToString());
        
        // Then
        Assert.That(domainEnvelopes, Is.Not.Null);
        Assert.That(domainEnvelopes, Has.Count.EqualTo(1));
        
        var domainEnvelope = domainEnvelopes[0];
        Assert.Multiple(() =>
        {
            Assert.That(domainEnvelope.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(domainEnvelope.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(domainEnvelope.EventData, Is.AssignableTo(typeof(TennisClubRegisteredEvent)));
        });
        
        var tennisClubRegisteredEvent = domainEnvelope.EventData as TennisClubRegisteredEvent;
        Assert.That(tennisClubRegisteredEvent, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(tennisClubRegisteredEvent.Name, Is.EqualTo(nameExpected));
            Assert.That(tennisClubRegisteredEvent.IsLocked, Is.EqualTo(isLockedExpected));
            Assert.That(tennisClubRegisteredEvent.SubscriptionTierId, Is.EqualTo(subscriptionTierIdExpected));
            Assert.That(tennisClubRegisteredEvent.MemberIds, Has.Count.EqualTo(memberIdsCountExpected));
        });
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
        var tennisClub = new TennisClub();
        
        var tennisClubRegisteredEvent =
            new TennisClubRegisteredEvent(tennisClubIdExpected, nameExpected, isLockedExpected,
                subscriptionTierIdExpected, memberIdsExpected);
        
        var domainEnvelope =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubIdExpected.Id,
                EventType.TENNIS_CLUB_REGISTERED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubRegisteredEvent);
        
        // When
        tennisClub.Apply(domainEnvelope);
        
        // Then
        Assert.Multiple(() =>
        {
            Assert.That(tennisClub.TennisClubId, Is.EqualTo(tennisClubIdExpected));
            Assert.That(tennisClub.Name, Is.EqualTo(nameExpected));
            Assert.That(tennisClub.IsLocked, Is.EqualTo(isLockedExpected));
            Assert.That(tennisClub.SubscriptionTierId, Is.EqualTo(subscriptionTierIdExpected));
            Assert.That(tennisClub.MemberIds, Is.EqualTo(memberIdsExpected));
        });
    }
    
    [Test]
    public void
        GivenUnlockedTennisClub_ProcessTennisClubLockCommand_ThenDomainEnvelopeWithTennisClubLockedEventIsReturned()
    {
        // Given
        var eventTypeExpected = EventType.TENNIS_CLUB_LOCKED;
        var entityTypeExpected = EntityType.TENNIS_CLUB;
        var name = "Test Tennis Club";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        tennisClub.ProcessTennisClubRegisterCommand(name, subscriptionTierId.Id.ToString())
            .ForEach(domainEvent => tennisClub.Apply(domainEvent));
        
        // When
        var domainEnvelopes = tennisClub.ProcessTennisClubLockCommand();
        
        // Then
        Assert.That(domainEnvelopes, Is.Not.Null);
        Assert.That(domainEnvelopes, Has.Count.EqualTo(1));
        
        var domainEnvelope = domainEnvelopes[0];
        Assert.Multiple(() =>
        {
            Assert.That(domainEnvelope.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(domainEnvelope.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(domainEnvelope.EventData, Is.AssignableTo(typeof(TennisClubLockedEvent)));
        });
    }
    
    [Test]
    public void GivenLockedTennisClub_ProcessTennisClubLockCommand_ThenExceptionIsThrown()
    {
        // Given
        var name = "Test Tennis Club";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        tennisClub.ProcessTennisClubRegisterCommand(name, subscriptionTierId.Id.ToString())
            .ForEach(domainEvent => tennisClub.Apply(domainEvent));
        tennisClub.ProcessTennisClubLockCommand().ForEach(domainEvent => tennisClub.Apply(domainEvent));
        
        // When ... Then
        Assert.Throws<InvalidOperationException>(() => tennisClub.ProcessTennisClubLockCommand());
    }
    
    [Test]
    public void GivenTennisClubLockedEvent_WhenApply_ThenTennisClubIsInExpectedState()
    {
        // Given
        var tennisClubId = new TennisClubId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        
        var tennisClubLockedEvent = new TennisClubLockedEvent();
        
        var domainEvent = new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
            EventType.TENNIS_CLUB_LOCKED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubLockedEvent);
        
        // When
        tennisClub.Apply(domainEvent);
        
        // Then
        Assert.That(tennisClub.IsLocked, Is.True);
    }
    
    [Test]
    public void
        GivenLockedTennisClub_WhenProcessTennisClubUnlockCommand_ThenDomainEnvelopeWithTennisClubUnlockedEventIsReturned()
    {
        // Given
        var eventTypeExpected = EventType.TENNIS_CLUB_UNLOCKED;
        var entityTypeExpected = EntityType.TENNIS_CLUB;
        var name = "Test Tennis Club";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        tennisClub.ProcessTennisClubRegisterCommand(name, subscriptionTierId.Id.ToString())
            .ForEach(domainEvent => tennisClub.Apply(domainEvent));
        tennisClub.ProcessTennisClubLockCommand()
            .ForEach(domainEvent => tennisClub.Apply(domainEvent));
        
        // When
        var domainEnvelopes = tennisClub.ProcessTennisClubUnlockCommand();
        
        // Then
        Assert.That(domainEnvelopes, Is.Not.Null);
        Assert.That(domainEnvelopes, Has.Count.EqualTo(1));
        
        var domainEnvelope = domainEnvelopes[0];
        Assert.Multiple(() =>
        {
            Assert.That(domainEnvelope.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(domainEnvelope.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(domainEnvelope.EventData, Is.AssignableTo(typeof(TennisClubUnlockedEvent)));
        });
    }
    
    [Test]
    public void GivenUnlockedTennisClub_ProcessTennisClubUnlockCommand_ThenExceptionIsThrown()
    {
        // Given
        var name = "Test Tennis Club";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        tennisClub.ProcessTennisClubRegisterCommand(name, subscriptionTierId.Id.ToString())
            .ForEach(domainEvent => tennisClub.Apply(domainEvent));
        
        // When ... Then
        Assert.Throws<InvalidOperationException>(() => tennisClub.ProcessTennisClubUnlockCommand());
    }
    
    [Test]
    public void GivenTennisClubUnlockedEvent_WhenApply_ThenTennisClubIsInExpectedState()
    {
        // Given
        var tennisClubId = new TennisClubId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        
        var tennisClubUnlockedEvent = new TennisClubUnlockedEvent();
        
        var domainEvent = new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
            EventType.TENNIS_CLUB_UNLOCKED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubUnlockedEvent);
        
        // When
        tennisClub.Apply(domainEvent);
        
        // Then
        Assert.That(tennisClub.IsLocked, Is.False);
    }
    
    [Test]
    public void
        GivenDifferentSubscriptionTierId_WhenProcessTennisClubChangeSubscriptionTierCommand_ThenDomainEnvelopeWithTennisClubSubscriptionTierChangedEventIsReturned()
    {
        // Given
        var eventTypeExpected = EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED;
        var entityTypeExpected = EntityType.TENNIS_CLUB;
        var name = "Test Tennis Club";
        var subscriptionTierIdInitial = new SubscriptionTierId(Guid.NewGuid());
        var subscriptionTierIdExpected = new SubscriptionTierId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        tennisClub.ProcessTennisClubRegisterCommand(name, subscriptionTierIdInitial.Id.ToString())
            .ForEach(domainEvent => tennisClub.Apply(domainEvent));
        
        // When
        var domainEnvelopes =
            tennisClub.ProcessTennisClubChangeSubscriptionTierCommand(subscriptionTierIdExpected.Id.ToString());
        
        // Then
        Assert.That(domainEnvelopes, Is.Not.Null);
        Assert.That(domainEnvelopes, Has.Count.EqualTo(1));
        
        var domainEnvelope = domainEnvelopes[0];
        Assert.Multiple(() =>
        {
            Assert.That(domainEnvelope.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(domainEnvelope.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(domainEnvelope.EventData, Is.AssignableTo(typeof(TennisClubSubscriptionTierChangedEvent)));
        });
        
        var tennisClubSubscriptionTierChangedEvent = domainEnvelope.EventData as TennisClubSubscriptionTierChangedEvent;
        Assert.That(tennisClubSubscriptionTierChangedEvent, Is.Not.Null);
        Assert.That(tennisClubSubscriptionTierChangedEvent.SubscriptionTierId, Is.EqualTo(subscriptionTierIdExpected));
    }
    
    [Test]
    public void GivenSameSubscriptionTierId_WhenProcessTennisClubChangeSubscriptionTierCommand_ThenExceptionIsThrown()
    {
        // Given
        var name = "Test Tennis Club";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        tennisClub.ProcessTennisClubRegisterCommand(name, subscriptionTierId.Id.ToString())
            .ForEach(domainEvent => tennisClub.Apply(domainEvent));
        
        // When ... Then
        Assert.Throws<InvalidOperationException>(() =>
            tennisClub.ProcessTennisClubChangeSubscriptionTierCommand(subscriptionTierId.Id.ToString()));
    }
    
    [Test]
    public void GivenTennisClubSubscriptionTierChangedEvent_WhenApply_ThenTennisClubIsInExpectedState()
    {
        // Given
        var tennisClubId = new TennisClubId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        
        var subscriptionTierIdExpected = new SubscriptionTierId(Guid.NewGuid());
        var tennisClubSubscriptionTierChangedEvent =
            new TennisClubSubscriptionTierChangedEvent(subscriptionTierIdExpected);
        
        var domainEvent = new DomainEnvelope<ITennisClubDomainEvent>(
            Guid.NewGuid(), tennisClubId.Id, EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED,
            EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubSubscriptionTierChangedEvent
        );
        
        // When
        tennisClub.Apply(domainEvent);
        
        // Then
        Assert.That(tennisClub.SubscriptionTierId, Is.EqualTo(subscriptionTierIdExpected));
    }
}