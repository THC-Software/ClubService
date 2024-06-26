using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.Enum;
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
        var subscriptionTierIdExpected = new SubscriptionTierId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        
        // When
        var domainEnvelopes =
            tennisClub.ProcessTennisClubRegisterCommand(nameExpected, subscriptionTierIdExpected);
        
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
            Assert.That(tennisClubRegisteredEvent.Status, Is.EqualTo(TennisClubStatus.ACTIVE));
            Assert.That(tennisClubRegisteredEvent.SubscriptionTierId, Is.EqualTo(subscriptionTierIdExpected));
        });
    }
    
    [Test]
    public void GivenTennisClubRegisteredEvent_WhenApply_ThenTennisClubIsInExpectedState()
    {
        // Given
        var tennisClubIdExpected = new TennisClubId(Guid.NewGuid());
        var nameExpected = "Test Tennis Club";
        var subscriptionTierIdExpected = new SubscriptionTierId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        
        var tennisClubRegisteredEvent =
            new TennisClubRegisteredEvent(tennisClubIdExpected, nameExpected,
                subscriptionTierIdExpected, TennisClubStatus.ACTIVE);
        
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
            Assert.That(tennisClub.Status, Is.EqualTo(TennisClubStatus.ACTIVE));
            Assert.That(tennisClub.SubscriptionTierId, Is.EqualTo(subscriptionTierIdExpected));
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
        tennisClub.ProcessTennisClubRegisterCommand(name, subscriptionTierId)
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
        tennisClub.ProcessTennisClubRegisterCommand(name, subscriptionTierId)
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
        Assert.That(tennisClub.Status, Is.EqualTo(TennisClubStatus.LOCKED));
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
        tennisClub.ProcessTennisClubRegisterCommand(name, subscriptionTierId)
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
        tennisClub.ProcessTennisClubRegisterCommand(name, subscriptionTierId)
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
        Assert.That(tennisClub.Status, Is.EqualTo(TennisClubStatus.ACTIVE));
    }
    
    [Test]
    public void
        GivenDifferentSubscriptionTierId_WhenProcessTennisClubUpdateCommand_ThenDomainEnvelopeWithTennisClubSubscriptionTierChangedEventIsReturned()
    {
        // Given
        var eventTypeExpected = EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED;
        var entityTypeExpected = EntityType.TENNIS_CLUB;
        var name = "Test Tennis Club";
        var subscriptionTierIdInitial = new SubscriptionTierId(Guid.NewGuid());
        var subscriptionTierIdExpected = new SubscriptionTierId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        tennisClub.ProcessTennisClubRegisterCommand(name, subscriptionTierIdInitial)
            .ForEach(domainEvent => tennisClub.Apply(domainEvent));
        
        // When
        var domainEnvelopes =
            tennisClub.ProcessTennisClubUpdateCommand(null, subscriptionTierIdExpected);
        
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
    public void GivenSameSubscriptionTierId_WhenProcessTennisClubUpdateCommand_ThenEmptyListIsReturned()
    {
        // Given
        var name = "Test Tennis Club";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        tennisClub.ProcessTennisClubRegisterCommand(name, subscriptionTierId)
            .ForEach(domainEvent => tennisClub.Apply(domainEvent));
        
        // When
        var domainEnvelopes = tennisClub.ProcessTennisClubUpdateCommand(null, subscriptionTierId);
        
        // Then
        Assert.That(domainEnvelopes, Is.Empty);
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
    
    [Test]
    public void
        GivenDifferentName_WhenProcessTennisClubUpdateCommand_ThenDomainEnvelopeWithTennisClubNameChangedEventIsReturned()
    {
        // Given
        var eventTypeExpected = EventType.TENNIS_CLUB_NAME_CHANGED;
        var entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubNameChangedEvent);
        var nameInitial = "Test Tennis Club";
        var nameExpected = "New Tennis Club Name";
        var subscriptionTierIdInitial = new SubscriptionTierId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        tennisClub.ProcessTennisClubRegisterCommand(nameInitial, subscriptionTierIdInitial)
            .ForEach(domainEvent => tennisClub.Apply(domainEvent));
        
        // When
        var domainEnvelopes =
            tennisClub.ProcessTennisClubUpdateCommand(nameExpected, null);
        
        // Then
        Assert.That(domainEnvelopes, Is.Not.Null);
        Assert.That(domainEnvelopes, Has.Count.EqualTo(1));
        
        var domainEnvelope = domainEnvelopes[0];
        Assert.Multiple(() =>
        {
            Assert.That(domainEnvelope.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(domainEnvelope.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(domainEnvelope.EventData, Is.AssignableTo(eventDataTypeExpected));
        });
        
        var tennisClubNameChangedEvent = domainEnvelope.EventData as TennisClubNameChangedEvent;
        Assert.That(tennisClubNameChangedEvent, Is.Not.Null);
        Assert.That(tennisClubNameChangedEvent.Name, Is.EqualTo(nameExpected));
    }
    
    [Test]
    public void GivenSameName_WhenProcessTennisClubUpdateCommand_ThenEmptyListIsReturned()
    {
        // Given
        var name = "Test Tennis Club";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        tennisClub.ProcessTennisClubRegisterCommand(name, subscriptionTierId)
            .ForEach(domainEvent => tennisClub.Apply(domainEvent));
        
        // When
        var domainEnvelopes = tennisClub.ProcessTennisClubUpdateCommand(name, null);
        
        // Then
        Assert.That(domainEnvelopes, Is.Empty);
    }
    
    [Test]
    public void GivenTennisClubNameChangedEvent_WhenApply_ThenTennisClubIsInExpectedState()
    {
        // Given
        var tennisClubId = new TennisClubId(Guid.NewGuid());
        var tennisClub = new TennisClub();
        
        var nameExpected = "Test";
        var tennisClubNameChangedEvent =
            new TennisClubNameChangedEvent(nameExpected);
        
        var domainEvent = new DomainEnvelope<ITennisClubDomainEvent>(
            Guid.NewGuid(), tennisClubId.Id, EventType.TENNIS_CLUB_NAME_CHANGED,
            EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubNameChangedEvent
        );
        
        // When
        tennisClub.Apply(domainEvent);
        
        // Then
        Assert.That(tennisClub.Name, Is.EqualTo(nameExpected));
    }
}