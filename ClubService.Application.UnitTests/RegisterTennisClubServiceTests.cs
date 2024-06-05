using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Application.Impl;
using ClubService.Domain.Event;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;
using Moq;

namespace ClubService.Application.UnitTests;

[TestFixture]
public class RegisterTennisClubServiceTests
{
    private Mock<IEventRepository> _eventRepositoryMock;
    private Mock<IEventStoreTransactionManager> _eventStoreTransactionManagerMock;
    private RegisterTennisClubService _registerTennisClubService;
    
    [SetUp]
    public void Setup()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _eventStoreTransactionManagerMock = new Mock<IEventStoreTransactionManager>();
        
        // set up the TransactionScope method to call the passed function
        _eventStoreTransactionManagerMock
            .Setup(mgr => mgr.TransactionScope(It.IsAny<Func<Task>>()))
            .Returns((Func<Task> transactionalOperation) => transactionalOperation());
        
        _registerTennisClubService = new RegisterTennisClubService(
            _eventRepositoryMock.Object,
            _eventStoreTransactionManagerMock.Object
        );
    }
    
    [Test]
    public async Task GivenValidInputs_WhenRegisterTennisClub_ThenRepoIsCalledWithExpectedEvent()
    {
        // Given
        const int eventCountExpected = 0;
        const EventType eventTypeExpected = EventType.TENNIS_CLUB_REGISTERED;
        const EntityType entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubRegisteredEvent);
        var subscriptionTierId = Guid.NewGuid();
        var tennisClubRegisterCommand =
            new TennisClubRegisterCommand("Test Tennis Club", subscriptionTierId);
        List<DomainEnvelope<ISubscriptionTierDomainEvent>> subscriptionTierDomainEvents =
        [
            new DomainEnvelope<ISubscriptionTierDomainEvent>(
                new Guid("8d4d3eff-b77b-4e21-963b-e211366bb94b"),
                subscriptionTierId,
                EventType.SUBSCRIPTION_TIER_CREATED,
                EntityType.SUBSCRIPTION_TIER,
                DateTime.UtcNow,
                new SubscriptionTierCreatedEvent(
                    new SubscriptionTierId(subscriptionTierId),
                    "Gold Subscription Tier",
                    200)
            )
        ];
        
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ISubscriptionTierDomainEvent>(subscriptionTierId))
            .ReturnsAsync(subscriptionTierDomainEvents);
        
        // When
        _ = await _registerTennisClubService.RegisterTennisClub(tennisClubRegisterCommand);
        
        // Then
        _eventRepositoryMock.Verify(repo =>
                repo.Append(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
                        e.EventType == eventTypeExpected &&
                        e.EntityType == entityTypeExpected &&
                        e.EventData.GetType() == eventDataTypeExpected),
                    eventCountExpected), Times.Once
        );
    }
    
    [Test]
    public void GivenNonExistentSubscriptionTierId_WhenRegisterTennisClub_ThenExceptionIsThrown()
    {
        // Given
        var subscriptionTierId = Guid.NewGuid();
        var tennisClubRegisterCommand =
            new TennisClubRegisterCommand("Test Tennis Club", subscriptionTierId);
        List<DomainEnvelope<ISubscriptionTierDomainEvent>> subscriptionTierDomainEvents = [];
        
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ISubscriptionTierDomainEvent>(subscriptionTierId))
            .ReturnsAsync(subscriptionTierDomainEvents);
        
        // When ... Then
        Assert.ThrowsAsync<SubscriptionTierNotFoundException>(async () =>
            await _registerTennisClubService.RegisterTennisClub(tennisClubRegisterCommand));
    }
}