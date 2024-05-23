using ClubService.Application.Api.Exceptions;
using ClubService.Application.Impl;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using Moq;

namespace ClubService.Application.UnitTests;

[TestFixture]
public class UpdateTennisClubServiceTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _updateTennisClubService = new UpdateTennisClubService(_eventRepositoryMock.Object);
    }
    
    private UpdateTennisClubService _updateTennisClubService;
    private Mock<IEventRepository> _eventRepositoryMock;
    
    [Test]
    public async Task GivenUnlockedTennisClub_WhenLockTennisClub_ThenRepoIsCalledWithExpectedEvent()
    {
        // Given
        var tennisClubId = new TennisClubId(Guid.NewGuid());
        var name = "Test Tennis Club";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        
        var tennisClubRegisteredEvent =
            new TennisClubRegisteredEvent(tennisClubId, name,
                subscriptionTierId, TennisClubStatus.NONE);
        var domainEnvelopeTennisClubRegistered =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_REGISTERED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubRegisteredEvent);
        
        var tennisClubLockedEvent = new TennisClubLockedEvent();
        var domainEnvelopeTennisClubLocked =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_LOCKED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubLockedEvent);
        
        var existingDomainEventsBeforeLock = new List<DomainEnvelope<ITennisClubDomainEvent>>
        {
            domainEnvelopeTennisClubRegistered
        };
        
        var existingDomainEventsAfterLock = new List<DomainEnvelope<ITennisClubDomainEvent>>
        {
            domainEnvelopeTennisClubRegistered,
            domainEnvelopeTennisClubLocked
        };
        
        _eventRepositoryMock.SetupSequence(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .ReturnsAsync(existingDomainEventsBeforeLock)
            .ReturnsAsync(existingDomainEventsAfterLock);
        
        // When
        _ = await _updateTennisClubService.LockTennisClub(tennisClubId.Id.ToString());
        
        // Then
        _eventRepositoryMock.Verify(repo => repo.Save(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
            e.EventType == EventType.TENNIS_CLUB_LOCKED &&
            e.EntityType == EntityType.TENNIS_CLUB &&
            e.EventData.GetType() == typeof(TennisClubLockedEvent))), Times.Once);
        _eventRepositoryMock.Verify(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id),
            Times.Exactly(2));
    }
    
    [Test]
    public void GivenNonExistentTennisClubId_WhenLockTennisClub_ThenExceptionIsThrown()
    {
        // Given
        var clubId = Guid.NewGuid().ToString();
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .ReturnsAsync(new List<DomainEnvelope<ITennisClubDomainEvent>>());
        
        // When ... Then
        Assert.ThrowsAsync<TennisClubNotFoundException>(() => _updateTennisClubService.LockTennisClub(clubId));
    }
    
    [Test]
    public async Task GivenLockedTennisClub_WhenUnlockTennisClub_ThenRepoIsCalledWithExpectedEvent()
    {
        // Given
        var tennisClubId = new TennisClubId(Guid.NewGuid());
        var name = "Test Tennis Club";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        
        var tennisClubRegisteredEvent =
            new TennisClubRegisteredEvent(tennisClubId, name,
                subscriptionTierId, TennisClubStatus.NONE);
        var domainEnvelopeTennisClubRegistered =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_REGISTERED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubRegisteredEvent);
        
        var tennisClubLockedEvent = new TennisClubLockedEvent();
        var domainEnvelopeTennisClubLocked =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_LOCKED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubLockedEvent);
        
        var tennisClubUnlockedEvent = new TennisClubUnlockedEvent();
        var domainEnvelopeTennisClubUnlocked =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_UNLOCKED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubUnlockedEvent);
        
        var existingDomainEventsBeforeUnlock = new List<DomainEnvelope<ITennisClubDomainEvent>>
        {
            domainEnvelopeTennisClubRegistered,
            domainEnvelopeTennisClubLocked
        };
        
        var existingDomainEventsAfterUnlock = new List<DomainEnvelope<ITennisClubDomainEvent>>
        {
            domainEnvelopeTennisClubRegistered,
            domainEnvelopeTennisClubLocked,
            domainEnvelopeTennisClubUnlocked
        };
        
        _eventRepositoryMock.SetupSequence(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .ReturnsAsync(existingDomainEventsBeforeUnlock)
            .ReturnsAsync(existingDomainEventsAfterUnlock);
        
        // When
        _ = await _updateTennisClubService.UnlockTennisClub(tennisClubId.Id.ToString());
        
        // Then
        _eventRepositoryMock.Verify(repo => repo.Save(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
            e.EventType == EventType.TENNIS_CLUB_UNLOCKED &&
            e.EntityType == EntityType.TENNIS_CLUB &&
            e.EventData.GetType() == typeof(TennisClubUnlockedEvent))), Times.Once);
        _eventRepositoryMock.Verify(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id),
            Times.Exactly(2));
    }
    
    [Test]
    public void GivenNonExistentTennisClubId_WhenUnlockTennisClub_ThenExceptionIsThrown()
    {
        // Given
        var clubId = Guid.NewGuid().ToString();
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .ReturnsAsync(new List<DomainEnvelope<ITennisClubDomainEvent>>());
        
        // When ... Then
        Assert.ThrowsAsync<TennisClubNotFoundException>(() => _updateTennisClubService.UnlockTennisClub(clubId));
    }
    
    [Test]
    public async Task GivenDifferentSubscriptionTierId_WhenChangeSubscriptionTier_ThenRepoIsCalledWithExpectedEvent()
    {
        // Given
        var tennisClubId = new TennisClubId(Guid.NewGuid());
        var name = "Test Tennis Club";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        var newSubscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        
        var tennisClubRegisteredEvent =
            new TennisClubRegisteredEvent(tennisClubId, name,
                subscriptionTierId, TennisClubStatus.NONE);
        var domainEnvelopeTennisClubRegistered =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_REGISTERED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubRegisteredEvent);
        
        var tennisClubSubscriptionTierChangedEvent =
            new TennisClubSubscriptionTierChangedEvent(newSubscriptionTierId);
        var domainEnvelopeTennisClubSubscriptionTierChanged =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED, EntityType.TENNIS_CLUB,
                DateTime.UtcNow, tennisClubSubscriptionTierChangedEvent);
        
        var existingDomainEventsBefore = new List<DomainEnvelope<ITennisClubDomainEvent>>
        {
            domainEnvelopeTennisClubRegistered
        };
        
        var existingDomainEventsAfter = new List<DomainEnvelope<ITennisClubDomainEvent>>
        {
            domainEnvelopeTennisClubRegistered,
            domainEnvelopeTennisClubSubscriptionTierChanged
        };
        
        _eventRepositoryMock.SetupSequence(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .ReturnsAsync(existingDomainEventsBefore)
            .ReturnsAsync(existingDomainEventsAfter);
        
        // When
        _ = await _updateTennisClubService.ChangeSubscriptionTier(tennisClubId.Id.ToString(),
            newSubscriptionTierId.Id.ToString());
        
        // Then
        _eventRepositoryMock.Verify(repo => repo.Save(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
            e.EventType == EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED &&
            e.EntityType == EntityType.TENNIS_CLUB &&
            e.EventData.GetType() == typeof(TennisClubSubscriptionTierChangedEvent))), Times.Once);
        _eventRepositoryMock.Verify(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id),
            Times.Exactly(2));
    }
    
    [Test]
    public void GivenNonExistentTennisClubId_WhenChangeSubscriptionTier_ThenExceptionIsThrown()
    {
        // Given
        var clubId = Guid.NewGuid().ToString();
        var subscriptionTierId = Guid.NewGuid().ToString();
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .ReturnsAsync(new List<DomainEnvelope<ITennisClubDomainEvent>>());
        
        // When ... Then
        Assert.ThrowsAsync<TennisClubNotFoundException>(() =>
            _updateTennisClubService.ChangeSubscriptionTier(clubId, subscriptionTierId));
    }
    
    [Test]
    public async Task GivenDifferentName_WhenName_ThenRepoIsCalledWithExpectedEvent()
    {
        // Given
        var tennisClubId = new TennisClubId(Guid.NewGuid());
        var name = "Test Tennis Club";
        var newName = "New Tennis Club Name";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        
        var tennisClubRegisteredEvent =
            new TennisClubRegisteredEvent(tennisClubId, name,
                subscriptionTierId, TennisClubStatus.NONE);
        var domainEnvelopeTennisClubRegistered =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_REGISTERED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubRegisteredEvent);
        
        var tennisClubNameChangedEvent =
            new TennisClubNameChangedEvent(newName);
        var domainEnvelopeTennisClubNameChanged =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_NAME_CHANGED, EntityType.TENNIS_CLUB,
                DateTime.UtcNow, tennisClubNameChangedEvent);
        
        var existingDomainEventsBefore = new List<DomainEnvelope<ITennisClubDomainEvent>>
        {
            domainEnvelopeTennisClubRegistered
        };
        
        var existingDomainEventsAfter = new List<DomainEnvelope<ITennisClubDomainEvent>>
        {
            domainEnvelopeTennisClubRegistered,
            domainEnvelopeTennisClubNameChanged
        };
        
        _eventRepositoryMock.SetupSequence(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .ReturnsAsync(existingDomainEventsBefore)
            .ReturnsAsync(existingDomainEventsAfter);
        
        // When
        _ = await _updateTennisClubService.ChangeName(tennisClubId.Id.ToString(),
            newName);
        
        // Then
        _eventRepositoryMock.Verify(repo => repo.Save(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
            e.EventType == EventType.TENNIS_CLUB_NAME_CHANGED &&
            e.EntityType == EntityType.TENNIS_CLUB &&
            e.EventData.GetType() == typeof(TennisClubNameChangedEvent))), Times.Once);
        _eventRepositoryMock.Verify(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id),
            Times.Exactly(2));
    }
    
    [Test]
    public void GivenNonExistentTennisClubId_WhenChangeName_ThenExceptionIsThrown()
    {
        // Given
        var clubId = Guid.NewGuid().ToString();
        var name = "Test";
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .ReturnsAsync(new List<DomainEnvelope<ITennisClubDomainEvent>>());
        
        // When ... Then
        Assert.ThrowsAsync<TennisClubNotFoundException>(() =>
            _updateTennisClubService.ChangeName(clubId, name));
    }
}