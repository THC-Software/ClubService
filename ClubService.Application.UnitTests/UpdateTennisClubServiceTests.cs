using ClubService.Application.Api.Exceptions;
using ClubService.Application.Impl;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;
using Moq;

namespace ClubService.Application.UnitTests;

[TestFixture]
public class UpdateTennisClubServiceTests
{
    private Mock<IEventRepository> _eventRepositoryMock;
    private Mock<IEventStoreTransactionManager> _eventStoreTransactionManagerMock;
    private UpdateTennisClubService _updateTennisClubService;
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _eventStoreTransactionManagerMock = new Mock<IEventStoreTransactionManager>();
        
        // set up the TransactionScope method to call the passed function
        _eventStoreTransactionManagerMock
            .Setup(mgr => mgr.TransactionScope(It.IsAny<Func<Task>>()))
            .Returns((Func<Task> transactionalOperation) => transactionalOperation());
        
        _updateTennisClubService = new UpdateTennisClubService(
            _eventRepositoryMock.Object,
            _eventStoreTransactionManagerMock.Object
        );
    }
    
    [Test]
    public async Task GivenUnlockedTennisClub_WhenLockTennisClub_ThenRepoIsCalledWithExpectedEvent()
    {
        // Given
        const int eventCountExpected = 1;
        const EventType eventTypeExpected = EventType.TENNIS_CLUB_LOCKED;
        const EntityType entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubLockedEvent);
        var tennisClubId = new TennisClubId(Guid.NewGuid());
        const string name = "Test Tennis Club";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        
        var tennisClubRegisteredEvent =
            new TennisClubRegisteredEvent(tennisClubId, name,
                subscriptionTierId, TennisClubStatus.ACTIVE);
        var domainEnvelopeTennisClubRegistered =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_REGISTERED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubRegisteredEvent);
        
        var existingDomainEvents = new List<DomainEnvelope<ITennisClubDomainEvent>>
        {
            domainEnvelopeTennisClubRegistered
        };
        
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .ReturnsAsync(existingDomainEvents);
        
        // When
        _ = await _updateTennisClubService.LockTennisClub(tennisClubId.Id.ToString());
        
        // Then
        _eventRepositoryMock.Verify(repo =>
                repo.Append(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
                        e.EventType == eventTypeExpected &&
                        e.EntityType == entityTypeExpected &&
                        e.EventData.GetType() == eventDataTypeExpected),
                    eventCountExpected), Times.Once
        );
        _eventRepositoryMock.Verify(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id),
            Times.Once);
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
        const int eventCountExpected = 2;
        const EventType eventTypeExpected = EventType.TENNIS_CLUB_UNLOCKED;
        const EntityType entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubUnlockedEvent);
        var tennisClubId = new TennisClubId(Guid.NewGuid());
        const string name = "Test Tennis Club";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        
        var tennisClubRegisteredEvent =
            new TennisClubRegisteredEvent(tennisClubId, name,
                subscriptionTierId, TennisClubStatus.ACTIVE);
        var domainEnvelopeTennisClubRegistered =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_REGISTERED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubRegisteredEvent);
        
        var tennisClubLockedEvent = new TennisClubLockedEvent();
        var domainEnvelopeTennisClubLocked =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_LOCKED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubLockedEvent);
        
        var existingDomainEvents = new List<DomainEnvelope<ITennisClubDomainEvent>>
        {
            domainEnvelopeTennisClubRegistered,
            domainEnvelopeTennisClubLocked
        };
        
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .ReturnsAsync(existingDomainEvents);
        
        // When
        _ = await _updateTennisClubService.UnlockTennisClub(tennisClubId.Id.ToString());
        
        // Then
        _eventRepositoryMock.Verify(repo =>
                repo.Append(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
                        e.EventType == eventTypeExpected &&
                        e.EntityType == entityTypeExpected &&
                        e.EventData.GetType() == eventDataTypeExpected),
                    eventCountExpected), Times.Once
        );
        _eventRepositoryMock.Verify(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id),
            Times.Once);
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
        const int eventCountExpected = 1;
        const EventType eventTypeExpected = EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED;
        const EntityType entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubSubscriptionTierChangedEvent);
        var tennisClubId = new TennisClubId(Guid.NewGuid());
        const string name = "Test Tennis Club";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        var newSubscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        
        var tennisClubRegisteredEvent =
            new TennisClubRegisteredEvent(tennisClubId, name,
                subscriptionTierId, TennisClubStatus.ACTIVE);
        var domainEnvelopeTennisClubRegistered =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_REGISTERED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubRegisteredEvent);
        
        var existingDomainEvents = new List<DomainEnvelope<ITennisClubDomainEvent>>
        {
            domainEnvelopeTennisClubRegistered
        };
        
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .ReturnsAsync(existingDomainEvents);
        
        // When
        _ = await _updateTennisClubService.ChangeSubscriptionTier(tennisClubId.Id.ToString(),
            newSubscriptionTierId.Id.ToString());
        
        // Then
        _eventRepositoryMock.Verify(repo =>
                repo.Append(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
                        e.EventType == eventTypeExpected &&
                        e.EntityType == entityTypeExpected &&
                        e.EventData.GetType() == eventDataTypeExpected),
                    eventCountExpected), Times.Once
        );
        _eventRepositoryMock.Verify(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id),
            Times.Once);
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
        const int eventCountExpected = 1;
        const EventType eventTypeExpected = EventType.TENNIS_CLUB_NAME_CHANGED;
        const EntityType entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubNameChangedEvent);
        var tennisClubId = new TennisClubId(Guid.NewGuid());
        const string name = "Test Tennis Club";
        const string newName = "New Tennis Club Name";
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        
        var tennisClubRegisteredEvent =
            new TennisClubRegisteredEvent(tennisClubId, name,
                subscriptionTierId, TennisClubStatus.ACTIVE);
        var domainEnvelopeTennisClubRegistered =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_REGISTERED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubRegisteredEvent);
        
        var existingDomainEvents = new List<DomainEnvelope<ITennisClubDomainEvent>>
        {
            domainEnvelopeTennisClubRegistered
        };
        
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .ReturnsAsync(existingDomainEvents);
        
        // When
        _ = await _updateTennisClubService.ChangeName(tennisClubId.Id.ToString(),
            newName);
        
        // Then
        _eventRepositoryMock.Verify(repo =>
                repo.Append(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
                        e.EventType == eventTypeExpected &&
                        e.EntityType == entityTypeExpected &&
                        e.EventData.GetType() == eventDataTypeExpected),
                    eventCountExpected), Times.Once
        );
        _eventRepositoryMock.Verify(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id),
            Times.Once);
    }
    
    [Test]
    public void GivenNonExistentTennisClubId_WhenChangeName_ThenExceptionIsThrown()
    {
        // Given
        var clubId = Guid.NewGuid().ToString();
        const string name = "Test";
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .ReturnsAsync(new List<DomainEnvelope<ITennisClubDomainEvent>>());
        
        // When ... Then
        Assert.ThrowsAsync<TennisClubNotFoundException>(() =>
            _updateTennisClubService.ChangeName(clubId, name));
    }
}