using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Application.Impl;
using ClubService.Domain.Event;
using ClubService.Domain.Event.SubscriptionTier;
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
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _transactionManagerMock = new Mock<ITransactionManager>();
        _loggerServiceMock = new Mock<ILoggerService<UpdateTennisClubService>>();

        // set up the TransactionScope method to call the passed function
        _transactionManagerMock
            .Setup(mgr => mgr.TransactionScope(It.IsAny<Func<Task>>()))
            .Returns((Func<Task> transactionalOperation) => transactionalOperation());

        _updateTennisClubService = new UpdateTennisClubService(
            _eventRepositoryMock.Object,
            _transactionManagerMock.Object,
            _loggerServiceMock.Object
        );
    }

    private Mock<IEventRepository> _eventRepositoryMock;
    private Mock<ITransactionManager> _transactionManagerMock;
    private Mock<ILoggerService<UpdateTennisClubService>> _loggerServiceMock;
    private UpdateTennisClubService _updateTennisClubService;

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

        _eventRepositoryMock.Setup(repo =>
                repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id, EntityType.TENNIS_CLUB))
            .ReturnsAsync(existingDomainEvents);

        // When
        _ = await _updateTennisClubService.LockTennisClub(tennisClubId.Id);

        // Then
        _eventRepositoryMock.Verify(repo =>
                repo.Append(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
                        e.EventType == eventTypeExpected &&
                        e.EntityType == entityTypeExpected &&
                        e.EventData.GetType() == eventDataTypeExpected),
                    eventCountExpected), Times.Once
        );
        _eventRepositoryMock.Verify(
            repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id, EntityType.TENNIS_CLUB),
            Times.Once);
    }

    [Test]
    public void GivenNonExistentTennisClubId_WhenLockTennisClub_ThenExceptionIsThrown()
    {
        // Given
        var clubId = Guid.NewGuid();
        _eventRepositoryMock
            .Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(clubId, EntityType.TENNIS_CLUB))
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

        _eventRepositoryMock.Setup(repo =>
                repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id, EntityType.TENNIS_CLUB))
            .ReturnsAsync(existingDomainEvents);

        // When
        _ = await _updateTennisClubService.UnlockTennisClub(tennisClubId.Id);

        // Then
        _eventRepositoryMock.Verify(repo =>
                repo.Append(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
                        e.EventType == eventTypeExpected &&
                        e.EntityType == entityTypeExpected &&
                        e.EventData.GetType() == eventDataTypeExpected),
                    eventCountExpected), Times.Once
        );
        _eventRepositoryMock.Verify(
            repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id, EntityType.TENNIS_CLUB),
            Times.Once);
    }

    [Test]
    public void GivenNonExistentTennisClubId_WhenUnlockTennisClub_ThenExceptionIsThrown()
    {
        // Given
        var clubId = Guid.NewGuid();
        _eventRepositoryMock
            .Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(clubId, EntityType.TENNIS_CLUB))
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
        var tennisClubUpdateCommand = new TennisClubUpdateCommand(null, newSubscriptionTierId.Id);

        var tennisClubRegisteredEvent =
            new TennisClubRegisteredEvent(tennisClubId, name,
                subscriptionTierId, TennisClubStatus.ACTIVE);
        var domainEnvelopeTennisClubRegistered =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_REGISTERED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubRegisteredEvent);
        var existingTennisClubDomainEvents = new List<DomainEnvelope<ITennisClubDomainEvent>>
        {
            domainEnvelopeTennisClubRegistered
        };

        var subscriptionTierCreatedEvent = new SubscriptionTierCreatedEvent(newSubscriptionTierId, "Test", 42);
        var domainEnvelopeSubscriptionTierCreated =
            new DomainEnvelope<ISubscriptionTierDomainEvent>(Guid.NewGuid(), newSubscriptionTierId.Id,
                EventType.SUBSCRIPTION_TIER_CREATED, EntityType.SUBSCRIPTION_TIER, DateTime.UtcNow,
                subscriptionTierCreatedEvent);
        var existingSubscriptionTierDomainEvents = new List<DomainEnvelope<ISubscriptionTierDomainEvent>>
        {
            domainEnvelopeSubscriptionTierCreated
        };

        _eventRepositoryMock.Setup(repo =>
                repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id, EntityType.TENNIS_CLUB))
            .ReturnsAsync(existingTennisClubDomainEvents);
        _eventRepositoryMock.Setup(repo =>
                repo.GetEventsForEntity<ISubscriptionTierDomainEvent>(newSubscriptionTierId.Id,
                    EntityType.SUBSCRIPTION_TIER))
            .ReturnsAsync(existingSubscriptionTierDomainEvents);

        // When
        _ = await _updateTennisClubService.UpdateTennisClub(tennisClubId.Id,
            tennisClubUpdateCommand);

        // Then
        _eventRepositoryMock.Verify(repo =>
                repo.Append(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
                        e.EventType == eventTypeExpected &&
                        e.EntityType == entityTypeExpected &&
                        e.EventData.GetType() == eventDataTypeExpected),
                    eventCountExpected), Times.Once
        );
        _eventRepositoryMock.Verify(
            repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id, EntityType.TENNIS_CLUB),
            Times.Once);
    }

    [Test]
    public void GivenNonExistentTennisClubId_WhenChangeSubscriptionTier_ThenExceptionIsThrown()
    {
        // Given
        var clubId = Guid.NewGuid();
        var tennisClubUpdateCommand = new TennisClubUpdateCommand(null, Guid.NewGuid());
        _eventRepositoryMock
            .Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(clubId, EntityType.TENNIS_CLUB))
            .ReturnsAsync(new List<DomainEnvelope<ITennisClubDomainEvent>>());

        // When ... Then
        Assert.ThrowsAsync<TennisClubNotFoundException>(() =>
            _updateTennisClubService.UpdateTennisClub(clubId, tennisClubUpdateCommand));
    }

    [Test]
    public async Task GivenDifferentName_WhenUpdateTennisClub_ThenRepoIsCalledWithExpectedEvent()
    {
        // Given
        const int eventCountExpected = 1;
        const EventType eventTypeExpected = EventType.TENNIS_CLUB_NAME_CHANGED;
        const EntityType entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubNameChangedEvent);
        var tennisClubId = new TennisClubId(Guid.NewGuid());
        const string name = "Test Tennis Club";
        var tennisClubUpdateCommand = new TennisClubUpdateCommand("New Name", null);
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

        _eventRepositoryMock.Setup(repo =>
                repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id, EntityType.TENNIS_CLUB))
            .ReturnsAsync(existingDomainEvents);

        // When
        _ = await _updateTennisClubService.UpdateTennisClub(tennisClubId.Id,
            tennisClubUpdateCommand);

        // Then
        _eventRepositoryMock.Verify(repo =>
                repo.Append(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
                        e.EventType == eventTypeExpected &&
                        e.EntityType == entityTypeExpected &&
                        e.EventData.GetType() == eventDataTypeExpected),
                    eventCountExpected), Times.Once
        );
        _eventRepositoryMock.Verify(
            repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id, EntityType.TENNIS_CLUB),
            Times.Once);
    }

    [Test]
    public void GivenNonExistentTennisClubId_WhenUpdateTennisClub_ThenExceptionIsThrown()
    {
        // Given
        var clubId = Guid.NewGuid();
        var tennisClubUpdateCommand = new TennisClubUpdateCommand("Test", null);
        _eventRepositoryMock
            .Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(clubId, EntityType.TENNIS_CLUB))
            .ReturnsAsync(new List<DomainEnvelope<ITennisClubDomainEvent>>());

        // When ... Then
        Assert.ThrowsAsync<TennisClubNotFoundException>(() =>
            _updateTennisClubService.UpdateTennisClub(clubId, tennisClubUpdateCommand));
    }
}